using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Common.Extensions;
using Common.Models;
using Common.UserInterface.ViewModels;
using Reporter.Models;

namespace Reporter.Utilities
{
    public class FileReportRenderer : IReportRenderer
    {
        private ProgramSettings ProgramSettings { get; }
        private string FolderPath => ProgramSettings.ReportFolder;
        public FileReportRenderer(ProgramSettings programSettings)
        {
            ProgramSettings = programSettings;
        }

        public async Task<List<AbstractReportEntry>> GetReports()
        {
            var results = (await Task.Run(() => Directory.EnumerateFiles(FolderPath, "Packager_*.report")))
                .Select(filename => new FileReportEntry
                {
                    Filename = filename,
                    Timestamp = GetTicksFromFilename(filename),
                    DisplayName = GetDisplayNameFromFilename(filename)
                })
                .ToList();

            return new List<AbstractReportEntry>(results);
        }

        private static long GetTicksFromFilename(string filename)
        {
            var withoutExtension = Path.GetFileNameWithoutExtension(filename).ToDefaultIfEmpty();
            var ticksText = withoutExtension.Split('_').Last();

            long result;
            return long.TryParse(ticksText, out result)
                ? result
                : 0;
        }

        private static string GetDisplayNameFromFilename(string filename)
        {
            var ticks = GetTicksFromFilename(filename);
            if (ticks <= 0)
            {
                return "Packager Session [unspecified date]";
            }

            return $"Packager Session: {new DateTime(ticks):MM/dd/yyyy hh:mm tt}";
        }

        private async Task<OperationReport> GetReport(AbstractReportEntry reportEntry)
        {
            var convertedEntry = reportEntry as FileReportEntry;
            if (convertedEntry == null)
            {
                return null;
            }

            return await Task.Run(() =>
                OperationReport.Read<PackagerReport>(
                    Path.Combine(FolderPath, convertedEntry.Filename)));
        }

        public bool CanRender(AbstractReportEntry report)
        {
            return report is FileReportEntry;
        }

        public async Task Render(AbstractReportEntry entry, ILogPanelViewModel viewModel)
        {
           
            var report = await GetReport(entry) as PackagerReport;
            if (report == null)
            {
                viewModel.InsertLine($"There are no reports in the {ProgramSettings.ReportFolder} folder.\n\nPlease select a different folder.");
                return;
            }
            viewModel.BeginSection("Summary", "Results Summary:");
            viewModel.InsertLine($"Started:   {report.Timestamp:MM/dd/yyyy hh:mm tt}");
            viewModel.InsertLine($"Completed: {report.Timestamp.Add(report.Duration):MM/dd/yyyy hh:mm tt}");
            viewModel.InsertLine($"Duration:  {report.Duration:hh\\:mm\\:ss}");
            viewModel.InsertLine("");
            viewModel.InsertLine($"Found {report.ObjectReports.Count.ToSingularOrPlural("object", "objects")} to process.");

            var inError = report.ObjectReports.Where(r => r.Succeeded == false).ToList();
            var success = report.ObjectReports.Where(r => r.Succeeded).ToList();

            LogObjectResults(success, $"Successfully processed {success.ToSingularOrPlural("object", "objects")}:", viewModel);
            LogObjectResults(inError, $"Could not process {inError.ToSingularOrPlural("object", "objects")}:", viewModel);
            viewModel.EndSection("Summary");
        }

        private static void LogObjectResults(List<PackagerObjectReport> results, string header, ILogPanelViewModel viewModel)
        {
            if (results.Any() == false)
            {
                return;
            }

            viewModel.BeginSection(header, header);

            foreach (var result in results)
            {
                viewModel.InsertLine($"{result.Barcode} ({result.Duration:hh\\:mm\\:ss})");
                if (result.Succeeded == false)
                {
                    viewModel.InsertLine("");
                    viewModel.InsertLine($"ERROR: {result.Issue}");
                }
            }

            viewModel.EndSection(header, header);
        }
    }
}