using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Common.Models;
using NSubstitute;
using NUnit.Framework;
using Packager.Models.ResultModels;
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

        private Dictionary<string, DurationResult> MockPackagerIssueResults { get; set; }
        private Dictionary<string, DurationResult> MockPackagerSucceededResults { get; set; }
        private Exception Exception { get; set; }
        private PackagerReport OperationReport { get; set; }
        private string ReportPath { get; set; }

        [SetUp]
        public virtual void BeforeEach()
        {
            XmlExporter = Substitute.For<IXmlExporter>();

            XmlExporter.When(action => action.ExportToFile(Arg.Any<PackagerReport>(), Arg.Any<string>()))
                    .Do(call =>
                {
                    OperationReport = call.Arg<PackagerReport>();
                    ReportPath = call.Arg<string>();
                });

            ProgramSettings = Substitute.For<IProgramSettings>();
            ProgramSettings.LogDirectoryName.Returns("Log Folder");
          
            MockPackagerIssueResults = new Dictionary<string, DurationResult>
            {
                {"1111111111111", new DurationResult(new DateTime(2017,1,1),  "issue") },
                {"2222222222222", DurationResult.Success(new DateTime(2017,1,1)) }
            };

            MockPackagerSucceededResults = new Dictionary<string, DurationResult>
            {
                {"1111111111111", DurationResult.Success(new DateTime(2017,1,1)) },
                {"2222222222222", DurationResult.Success(new DateTime(2017,1,1)) }
            };

            Exception = new Exception("issue");

            ReportWriter = new ReportWriter(ProgramSettings, XmlExporter);
        }

        [Test]
        public void ItShouldCallXmlExporterWithCorrectPath()
        {
            var expected = Path.Combine(
                ProgramSettings.LogDirectoryName,
                $"Packager_{new DateTime(2017, 1, 1).Ticks:D19}.operationReport");

            ReportWriter.WriteResultsReport(MockPackagerSucceededResults, new DateTime(2017,1,1));
            Assert.That(ReportPath, Is.EqualTo(expected));
        }

        [Test]
        public void TimestampShouldBeSet()
        {
            ReportWriter.WriteResultsReport(MockPackagerSucceededResults, new DateTime(2017,1,1));
            Assert.That(OperationReport.Timestamp, Is.EqualTo(new DateTime(2017, 1, 1)));
        }


        [Test]
        public void DurationShouldBeSet()
        {
            ReportWriter.WriteResultsReport(MockPackagerSucceededResults, new DateTime(2017, 1, 1));
            Assert.That(OperationReport.Duration, Is.GreaterThan(new TimeSpan()));
        }


        [Test]
        public void IfPackagerIssuesSucceededShouldBeFalse()
        {
            ReportWriter.WriteResultsReport(MockPackagerIssueResults, new DateTime(2017, 1, 1));
            Assert.That(OperationReport.Succeeded, Is.EqualTo(false));
        }

        [Test]
        public void IfNoPackagerIssuesSucceededShouldBeTrue()
        {
            ReportWriter.WriteResultsReport(MockPackagerSucceededResults, new DateTime(2017, 1, 1));
            Assert.That(OperationReport.Succeeded, Is.EqualTo(true));
        }

        [Test]
        public void IfNoPackagerIssuesIssueShouldNotBeSet()
        {
            ReportWriter.WriteResultsReport(MockPackagerSucceededResults, new DateTime(2017, 1, 1));
            Assert.That(OperationReport.Issue, Is.EqualTo(string.Empty));
        }

        [Test]
        public void IfNoPackagerIssuesIssueShouldBeSet()
        {
            ReportWriter.WriteResultsReport(MockPackagerIssueResults, new DateTime(2017, 1, 1));
            Assert.That(OperationReport.Issue, Is.EqualTo("Issues occurred while processing one or more objects"));
        }

        [Test]
        public void ObjectResultsShouldBeCorrectIfIssuesOccur()
        {
            ReportWriter.WriteResultsReport(MockPackagerIssueResults, new DateTime(2017, 1, 1));
            AssertObjectReportsOk(MockPackagerIssueResults, OperationReport.ObjectReports);
        }

        [Test]
        public void ObjectResultsShouldBeCorrectIfNoIssuesOccur()
        {
            ReportWriter.WriteResultsReport(MockPackagerSucceededResults, new DateTime(2017, 1, 1));
            AssertObjectReportsOk(MockPackagerSucceededResults, OperationReport.ObjectReports);
        }

        [Test]
        public void SucceededShouldBeFalseWhenWritingExceptionReport()
        {
            ReportWriter.WriteResultsReport(Exception);
            Assert.That(OperationReport.Succeeded, Is.False);
        }

        [Test]
        public void IssueShouldBeCorrectWhenWritingExceptionReport()
        {
            ReportWriter.WriteResultsReport(Exception);
            Assert.That(OperationReport.Issue, Is.EqualTo(Exception.Message));
        }

        private static void AssertObjectReportsOk(Dictionary<string, DurationResult> originals,
            List<PackagerObjectReport> reports)
        {
            Assert.That(reports.Count, Is.EqualTo(originals.Keys.Count));

            foreach (var key in originals.Keys)
            {
                var report = reports.SingleOrDefault(r => r.Barcode.Equals(key));
                Assert.That(report, Is.Not.Null);

                Assert.That(report.Timestamp, Is.EqualTo(originals[key].Timestamp));
                Assert.That(report.Succeeded, Is.EqualTo(originals[key].Succeeded));
                Assert.That(report.Issue, Is.EqualTo(originals[key].Issue));
                Assert.That(report.Duration, Is.EqualTo(originals[key].Duration));
            }
        }

    }
}

