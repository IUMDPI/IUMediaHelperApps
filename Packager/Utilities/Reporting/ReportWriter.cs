using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Common.Extensions;
using Common.Models;
using Packager.Models.ResultModels;
using Packager.Models.SettingsModels;
using Packager.Utilities.Xml;

namespace Packager.Utilities.Reporting
{
    public class ReportWriter : IReportWriter
    {
        private IXmlExporter Exporter { get; }
        private string LogDirectoryName { get; }

        public ReportWriter(IProgramSettings programSettings, IXmlExporter exporter)
        {
            Exporter = exporter;
            LogDirectoryName = programSettings.LogDirectoryName;
        }

        public void WriteResultsReport(Dictionary<string, DurationResult> results, DateTime startTime)
        {
            var succeeded = results.Any(r => r.Value.Succeeded) && !results.Any(r=>r.Value.Failed); // if no failed results and at least 1 success result, set succeeded to true
            var skipped = results.Any(r => r.Value.Skipped); // if any skipped, set skipped to true
            var report = new PackagerReport
            {
                Timestamp = startTime,
                Duration = DateTime.Now - startTime,
                Succeeded = succeeded,
                Skipped = skipped,
                Issue = GetOverallIssue(results),
                ObjectReports = results.Select(r => new PackagerObjectReport
                {
                    Barcode = r.Key,
                    Succeeded = r.Value.Succeeded,
                    Skipped = r.Value.Skipped,
                    Failed = r.Value.Failed,
                    Issue = r.Value.Issue,
                    Timestamp = r.Value.Timestamp,
                    Duration = r.Value.Duration
                }).ToList()
            };

            Write(report);
        }

        private static string GetOverallIssue(Dictionary<string, DurationResult> results)
        {
            if (results.All(r => r.Value.Succeeded))
            {
                return string.Empty;
            }

            var lines = new List<string>();

            if (results.Any(r => r.Value.Failed))
            {
                lines.Add("Issues occurred while processing one or more objects.");
            }

            var skipped = results.Where(r => r.Value.Skipped).Select(r => r.Key).ToList();
            if (skipped.Any())
            {
                lines.Add($"Processing deferred for {skipped.ToPrettyDelimitedList()}.");
            }

            return string.Join("\n", lines);
        }

        public void WriteResultsReport(Exception e)
        {
            var report = new PackagerReport
            {
                Succeeded = false,
                Issue = e.Message,
                Timestamp = DateTime.Now
            };

            Write(report);
        }

        private void Write(AbstractOperationReport operationReport)
        {
            var reportPath = Path.Combine(LogDirectoryName, $"Packager_{operationReport.Timestamp.Ticks:D19}.operationReport");
            Exporter.ExportToFile(operationReport, reportPath);
        }
    }
}