using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using NSubstitute;
using NUnit.Framework;
using Packager.Factories;
using Packager.Models.FileModels;
using Packager.Models.ResultModels;
using Packager.Utilities.Bext;
using Packager.Utilities.ProcessRunners;

namespace Packager.Test.Utilities
{
    [TestFixture]
    public class BwfMetaEditRunnerTests
    {
        [SetUp]
        public virtual void BeforeEach()
        {
            ProductionFileModel = FileModelFactory.GetModel(ProductionFileName);
            PreservationFileModel = FileModelFactory.GetModel(PreservationFileName);
            PreservationIntermediateFileModel = FileModelFactory.GetModel(PreservationIntermediateFileName);

            ProcessRunner = Substitute.For<IProcessRunner>();
            ProcessRunner.Run(null).ReturnsForAnyArgs(x =>
            {
                var result = Substitute.For<IProcessResult>();
                result.ExitCode = 0;
                return Task.FromResult(result);
            });

            Instances = new List<AbstractFile>
            {
                PreservationFileModel,
                PreservationIntermediateFileModel,
                ProductionFileModel
            };
        }

        private const string BwfMetaEditPath = "bwfmetaedit.exe";
        private const string BaseProcessingDirectory = "processing";

        private const string ProductionFileName = "MDPI_4890764553278906_01_prod.wav";
        private const string PreservationIntermediateFileName = "MDPI_4890764553278906_01_presInt.wav";
        private const string PreservationFileName = "MDPI_4890764553278906_01_pres.wav";

        private IProcessRunner ProcessRunner { get; set; }

        private AbstractFile ProductionFileModel { get; set; }
        private AbstractFile PreservationFileModel { get; set; }
        private AbstractFile PreservationIntermediateFileModel { get; set; }

        private List<AbstractFile> Instances { get; set; }

        private ProcessStartInfo StartInfo { get; set; }

        public class WhenClearingMetadata : BwfMetaEditRunnerTests
        {
            public override async void BeforeEach()
            {
                base.BeforeEach();

                FieldsToClear = new List<BextFields> {BextFields.IARL, BextFields.ICRD, BextFields.IGNR};
                var runner = new BwfMetaEditRunner(ProcessRunner, BwfMetaEditPath, BaseProcessingDirectory, CancellationToken.None);
                await runner.ClearMetadata(ProductionFileModel, FieldsToClear);
                StartInfo = ProcessRunner.ReceivedCalls().First().GetArguments()[0] as ProcessStartInfo;
                Assert.That(StartInfo, Is.Not.Null);
            }

            private List<BextFields> FieldsToClear { get; set; }

            [Test]
            public void ArgsShouldIncludeEmptyFieldValues()
            {
                foreach (var field in FieldsToClear)
                {
                    Assert.That(StartInfo.Arguments.Contains($"{field}=\"\""));
                }
            }

            [Test]
            public void ArgsShouldIncludeVerbose()
            {
                Assert.That(StartInfo.Arguments.Contains("--verbose"));
            }

            [Test]
            public void ItShouldUseCorrectUtilityPath()
            {
                Assert.That(StartInfo.FileName, Is.EqualTo(BwfMetaEditPath));
            }
        }
    }
}