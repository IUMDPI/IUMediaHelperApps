using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Common.Models;
using Packager.Models.SettingsModels;
using Packager.Utilities.Xml;
using Packager.Validators;

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

        public void WriteResultsReport(Dictionary<string, ValidationResult> results)
        {
            var succeeded = results.All(r => r.Value.Result);
            var report = new PackagerReport
            {
                Timestamp = DateTime.Now,
                Succeeded = succeeded,
                Issue = succeeded ? string.Empty : "Issues occurred while processing one or more objects",
                ObjectReports = results.Select(r => new PackagerObjectReport
                {
                    Barcode = r.Key,
                    Succeeded = r.Value.Result,
                    Issue = r.Value.Issue,
                    Timestamp = r.Value.Timestamp
                }).ToList()
            };

            Write(report);
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

        private void Write(PackagerReport report)
        {
            var reportPath = Path.Combine(LogDirectoryName, $"Packager_{DateTime.Now.Ticks:D19}.xml");
            Exporter.ExportToFile(report, reportPath);
        }
    }
}