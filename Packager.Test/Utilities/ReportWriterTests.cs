using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Common.Models;
using NSubstitute;
using NUnit.Framework;
using Packager.Models.SettingsModels;
using Packager.Utilities.Reporting;
using Packager.Utilities.Xml;
using Packager.Validators;

namespace Packager.Test.Utilities
{
    [TestFixture]
    public class ReportWriterTests
    {
        private IXmlExporter XmlExporter { get; set; }
        private IProgramSettings ProgramSettings { get; set; }
       
        private IReportWriter ReportWriter { get; set; }

        private Dictionary<string, ValidationResult> MockPackagerIssueResults { get; set; }
        private Dictionary<string, ValidationResult> MockPackagerSucceededResults { get; set; }
        private Exception Exception { get; set; }
        private PackagerReport Report { get; set; }
        private string ReportPath { get; set; }

        [SetUp]
        public virtual void BeforeEach()
        {
            XmlExporter = Substitute.For<IXmlExporter>();

            XmlExporter.When(action => action.ExportToFile(Arg.Any<PackagerReport>(), Arg.Any<string>()))
                    .Do(call =>
                {
                    Report = call.Arg<PackagerReport>();
                    ReportPath = call.Arg<string>();
                });

            ProgramSettings = Substitute.For<IProgramSettings>();
            ProgramSettings.LogDirectoryName.Returns("Log Folder");
          
            MockPackagerIssueResults = new Dictionary<string, ValidationResult>
            {
                {"1111111111111", new ValidationResult("issue") },
                {"2222222222222", ValidationResult.Success }
            };

            MockPackagerSucceededResults = new Dictionary<string, ValidationResult>
            {
                {"1111111111111", ValidationResult.Success },
                {"2222222222222", ValidationResult.Success }
            };

            Exception = new Exception("issue");

            ReportWriter = new ReportWriter(ProgramSettings, XmlExporter);
        }

        [Test]
        public void ItShouldCallXmlExporterWithCorrectPath()
        {
            ReportWriter.WriteResultsReport(MockPackagerSucceededResults);
            Assert.That(ReportPath.StartsWith(Path.Combine(ProgramSettings.LogDirectoryName, "Packager_")));
            Assert.That(ReportPath.EndsWith(".xml"));
        }

        [Test]
        public void TimestampShouldBeSet()
        {
            ReportWriter.WriteResultsReport(MockPackagerSucceededResults);
            Assert.That(Report.Timestamp, Is.GreaterThan(new DateTime()));
        }

        [Test]
        public void IfPackagerIssuesSucceededShouldBeFalse()
        {
            ReportWriter.WriteResultsReport(MockPackagerIssueResults);
            Assert.That(Report.Succeeded, Is.EqualTo(false));
        }

        [Test]
        public void IfNoPackagerIssuesSucceededShouldBeTrue()
        {
            ReportWriter.WriteResultsReport(MockPackagerSucceededResults);
            Assert.That(Report.Succeeded, Is.EqualTo(true));
        }

        [Test]
        public void IfNoPackagerIssuesIssueShouldNotBeSet()
        {
            ReportWriter.WriteResultsReport(MockPackagerSucceededResults);
            Assert.That(Report.Issue, Is.EqualTo(string.Empty));
        }

        [Test]
        public void IfNoPackagerIssuesIssueShouldBeSet()
        {
            ReportWriter.WriteResultsReport(MockPackagerIssueResults);
            Assert.That(Report.Issue, Is.EqualTo("Issues occurred while processing one or more objects"));
        }

        [Test]
        public void ObjectResultsShouldBeCorrectIfIssuesOccur()
        {
            ReportWriter.WriteResultsReport(MockPackagerIssueResults);
            AssertObjectReportsOk(MockPackagerIssueResults, Report.ObjectReports);
        }

        [Test]
        public void ObjectResultsShouldBeCorrectIfNoIssuesOccur()
        {
            ReportWriter.WriteResultsReport(MockPackagerSucceededResults);
            AssertObjectReportsOk(MockPackagerSucceededResults, Report.ObjectReports);
        }

        [Test]
        public void SucceededShouldBeFalseWhenWritingExceptionReport()
        {
            ReportWriter.WriteResultsReport(Exception);
            Assert.That(Report.Succeeded, Is.False);
        }

        [Test]
        public void IssueShouldBeCorrectWhenWritingExceptionReport()
        {
            ReportWriter.WriteResultsReport(Exception);
            Assert.That(Report.Issue, Is.EqualTo(Exception.Message));
        }

        private static void AssertObjectReportsOk(Dictionary<string, ValidationResult> originals,
            List<PackagerObjectReport> reports)
        {
            Assert.That(reports.Count, Is.EqualTo(originals.Keys.Count));

            foreach (var key in originals.Keys)
            {
                var report = reports.SingleOrDefault(r => r.Barcode.Equals(key));
                Assert.That(report, Is.Not.Null);

                Assert.That(report.Timestamp, Is.EqualTo(originals[key].Timestamp));
                Assert.That(report.Succeeded, Is.EqualTo(originals[key].Result));
                Assert.That(report.Issue, Is.EqualTo(originals[key].Issue));
            }
        }

    }
}

