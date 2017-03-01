using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.Threading.Tasks;
using Common.TaskScheduler.Schedulers;
using Common.UserInterface.ViewModels;
using Reporter.Models;

namespace Reporter.Utilities
{
    public class TaskSchedulerRenderer:IReportRenderer
    {
        private ILogPanelViewModel ViewModel { get; }

        public TaskSchedulerRenderer(ILogPanelViewModel viewModel)
        {
            ViewModel = viewModel;
        }

        public async Task<List<AbstractReportEntry>> GetReports()
        {
            var results = new List<AbstractReportEntry>();

            var tasks = new PackagerScheduler().GetExistingScheduledTasks().Select(t=>t);

            return tasks.Aggregate(results, (current, task) => current.Concat(GetEventRecords(task.Name, task.Path)).ToList());
        }

        private static IEnumerable<TaskSchedulerEntry> GetEventRecords(string name, string path)
        {
            const string source = "Microsoft-Windows-TaskScheduler/Operational";
            var queryText = $"*[EventData[Data[@Name='TaskName'] and (Data='{path}')]" +
                " and System[(Level=1 or Level=2 or Level=3)]]";

            var query = new EventLogQuery(source, PathType.LogName, queryText);
            var results = new List<TaskSchedulerEntry>();

            using (var reader = new EventLogReader(query))
            {
                for (var record = reader.ReadEvent(); record != null; record = reader.ReadEvent())
                {
                    var entry = new TaskSchedulerEntry
                    {
                        DisplayName = $"Task Scheduler: {record.TaskDisplayName}",
                        Timestamp = record.TimeCreated?.Ticks ?? 0L,
                        Contents = record.FormatDescription(),
                        TaskName = name
                    };
                    results.Add(entry);
                }
            }

            return results;
        }
        
        public bool CanRender(AbstractReportEntry report)
        {
            return report is TaskSchedulerEntry;
        }
        
        public async Task Render(AbstractReportEntry reportEntry)
        {
            var entry = reportEntry as TaskSchedulerEntry;
            if (entry == null)
            {
                ViewModel.InsertLine("This report could not be read.");
                return;
            }

           
            ViewModel.BeginSection("Summary", "Results Summary:");
            ViewModel.InsertLine($"Task Name: {entry.TaskName}");
            ViewModel.InsertLine($"Timestamp: {new DateTime(entry.Timestamp):MM/dd/yyyy hh:mm tt}");
            ViewModel.InsertLine("");
            ViewModel.InsertLine(GetIssueLines(entry.Contents));
            ViewModel.EndSection("Summary");
        }

        private static string GetIssueLines(string issue)
        {
            if (string.IsNullOrWhiteSpace(issue))
            {
                return "";
            }

            var charCount = 0;
            var lines = issue.Split(new[] { " " }, StringSplitOptions.RemoveEmptyEntries)
                            .GroupBy(w => (charCount += w.Length + 1) / 60)
                            .Select(g => string.Join(" ", g));

            return string.Join("\n", lines.ToArray());
        }
    }
}
