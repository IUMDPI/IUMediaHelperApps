using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using NSubstitute;
using NUnit.Framework;
using Packager.Models.FileModels;
using Packager.Models.ResultModels;
using Packager.Utilities;

namespace Packager.Test.Utilities
{
    [TestFixture]
    public class BwfMetaEditRunnerTests
    {
        [SetUp]
        public virtual void BeforeEach()
        {
            ProductionFileModel = new ObjectFileModel(ProductionFileName);
            PreservationFileModel = new ObjectFileModel(PreservationFileName);
            PreservationIntermediateFileModel = new ObjectFileModel(PreservationIntermediateFileName);

            ProcessRunner = Substitute.For<IProcessRunner>();
            ProcessRunner.Run(null).ReturnsForAnyArgs(x =>
            {
                var result = Substitute.For<IProcessResult>();
                result.ExitCode = 0;
                return Task.FromResult(result);
            });

            Instances = new List<ObjectFileModel> {PreservationFileModel, PreservationIntermediateFileModel, ProductionFileModel};
        }

        private const string BwfMetaEditPath = "bwfmetaedit.exe";
        private const string BaseProcessingDirectory = "processing";

        private const string ProductionFileName = "MDPI_4890764553278906_01_prod.wav";
        private const string PreservationIntermediateFileName = "MDPI_4890764553278906_01_pres-int.wav";
        private const string PreservationFileName = "MDPI_4890764553278906_01_pres.wav";

        private IProcessRunner ProcessRunner { get; set; }

        private ObjectFileModel ProductionFileModel { get; set; }
        private ObjectFileModel PreservationFileModel { get; set; }
        private ObjectFileModel PreservationIntermediateFileModel { get; set; }

        private List<ObjectFileModel> Instances { get; set; }

        private ProcessStartInfo StartInfo { get; set; }

        public class WhenClearingMetadata : BwfMetaEditRunnerTests
        {
            public override async void BeforeEach()
            {
                base.BeforeEach();

                FieldsToClear = new List<BextFields> {BextFields.IARL, BextFields.ICRD, BextFields.IGNR};
                var runner = new BwfMetaEditRunner(ProcessRunner, BwfMetaEditPath, BaseProcessingDirectory);
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