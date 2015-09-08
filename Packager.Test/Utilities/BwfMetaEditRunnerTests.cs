using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using NSubstitute;
using NUnit.Framework;
using Packager.Attributes;
using Packager.Extensions;
using Packager.Models.BextModels;
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

                var runner = new BwfMetaEditRunner(ProcessRunner, BwfMetaEditPath, BaseProcessingDirectory);
                await runner.ClearMetadata(ProductionFileModel);
                StartInfo = ProcessRunner.ReceivedCalls().First().GetArguments()[0] as ProcessStartInfo;
                Assert.That(StartInfo, Is.Not.Null);
            }

            [Test]
            public void ArgsShouldIncludeAppend()
            {
                Assert.That(StartInfo.Arguments.Contains("--append"));
            }

            [Test]
            public void ArgsShouldIncludeEmptyFieldValues()
            {
                foreach (var property in typeof (ConformancePointDocumentFileCore).GetProperties().Where(p => p.GetCustomAttribute<BextFieldAttribute>() != null))
                {
                    var attribute = property.GetCustomAttribute<BextFieldAttribute>();

                    Assert.That(StartInfo.Arguments.Contains($"--{attribute.Field}=\"\""), $"{attribute.Field} should be set correctly");
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

        public class WhenEmbeddingMetadata : BwfMetaEditRunnerTests
        {
            public override async void BeforeEach()
            {
                base.BeforeEach();

                Core = GenerateTestingCore();
                var runner = new BwfMetaEditRunner(ProcessRunner, BwfMetaEditPath, BaseProcessingDirectory);
                await runner.AddMetadata(ProductionFileModel, Core);
                StartInfo = ProcessRunner.ReceivedCalls().First().GetArguments()[0] as ProcessStartInfo;
                Assert.That(StartInfo, Is.Not.Null);
            }

            private ConformancePointDocumentFileCore Core { get; set; }

            private static ConformancePointDocumentFileCore GenerateTestingCore()
            {
                var result = new ConformancePointDocumentFileCore();
                foreach (var property in result.GetType().GetProperties().Where(p => p.GetCustomAttribute<BextFieldAttribute>() != null))
                {
                    property.SetValue(result, $"{property.Name} value");
                }

                return result;
            }

            [Test]
            public void ArgsShouldEndWithCorrectPath()
            {
                Assert.That(StartInfo.Arguments.EndsWith(Path.Combine(BaseProcessingDirectory, ProductionFileModel.GetFolderName(), ProductionFileModel.ToFileName()).ToQuoted()));
            }

            [Test]
            public void ArgsShouldIncludeAppend()
            {
                Assert.That(StartInfo.Arguments.Contains("--append"));
            }

            [Test]
            public void ArgsShouldIncludeCorrectFieldValues()
            {
                foreach (var property in Core.GetType().GetProperties().Where(p => p.GetCustomAttribute<BextFieldAttribute>() != null))
                {
                    var attribute = property.GetCustomAttribute<BextFieldAttribute>();

                    Assert.That(StartInfo.Arguments.Contains($"--{attribute.Field}=\"{property.GetValue(Core)}\""), $"{attribute.Field} should be set correctly");
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