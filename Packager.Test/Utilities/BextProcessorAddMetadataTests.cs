using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NSubstitute;
using NUnit.Framework;
using Packager.Extensions;
using Packager.Factories;
using Packager.Models.BextModels;
using Packager.Models.FileModels;
using Packager.Models.PodMetadataModels;
using Packager.Models.ResultModels;
using Packager.Observers;
using Packager.Utilities;
using Packager.Verifiers;

namespace Packager.Test.Utilities
{
    [TestFixture]
    public class BextProcessorAddMetadataTests
    {
        private const string BwfMetaEditPath = "bwfmetaedit.exe";
        private const string ProductionFileName = "MDPI_4890764553278906_01_prod.wav";
        private const string PreservationIntermediateFileName = "MDPI_4890764553278906_01_pres-int.wav";
        private const string PreservationFileName = "MDPI_4890764553278906_01_pres.wav";
        private const string BaseProcessingFolder = "base";
        private const string ExpectedProcessingFolder = "MDPI_4890764553278906";
        private const string Output = "Output";
        private BextProcessor BextProcessor { get; set; }
        private IProcessRunner ProcessRunner { get; set; }
        private IXmlExporter XmlExporter { get; set; }
        private IBwfMetaEditResultsVerifier Verifier { get; set; }
        private IConformancePointDocumentFactory ConformancePointDocumentFactory { get; set; }
        private IObserverCollection Observers { get; set; }
        private ObjectFileModel ProdFileModel { get; set; }
        private ObjectFileModel PresFileModel { get; set; }
        private ObjectFileModel PresIntFileModel { get; set; }
        private DigitalFileProvenance PreservationProvenance { get; set; }
        private DigitalFileProvenance PreservationIntermediateProvenance { get; set; }
        private DigitalFileProvenance ProductionProvenance { get; set; }
        private List<ObjectFileModel> Instances { get; set; }
        private ConsolidatedPodMetadata Metadata { get; set; }

        protected virtual void DoCustomSetup()
        {
        }

        private static DigitalFileProvenance GetFileProvenance(string filename)
        {
            return new DigitalFileProvenance {Filename = filename};
        }

        [SetUp]
        public async void BeforeEach()
        {
            ProdFileModel = new ObjectFileModel(ProductionFileName);
            PresFileModel = new ObjectFileModel(PreservationFileName);
            PresIntFileModel = new ObjectFileModel(PreservationIntermediateFileName);

            PreservationProvenance = GetFileProvenance(PreservationFileName);
            PreservationIntermediateProvenance = GetFileProvenance(PreservationIntermediateFileName);
            ProductionProvenance = GetFileProvenance(ProductionFileName);

            Verifier = Substitute.For<IBwfMetaEditResultsVerifier>();
            ProcessRunner = Substitute.For<IProcessRunner>();
            ProcessRunner.Run(Arg.Any<ProcessStartInfo>()).Returns(
                Task.FromResult((IProcessResult) new ProcessResult {ExitCode = 0, StandardError = Output, StandardOutput = Output}));

            XmlExporter = Substitute.For<IXmlExporter>();
            Observers = Substitute.For<IObserverCollection>();


            Metadata = new ConsolidatedPodMetadata();

            ConformancePointDocumentFactory = Substitute.For<IConformancePointDocumentFactory>();
            ConformancePointDocumentFactory.Generate(Arg.Any<ObjectFileModel>(), Arg.Any<DigitalFileProvenance>(), Metadata)
                .Returns(r => new ConformancePointDocumentFile
                {
                    Name = Path.Combine(ExpectedProcessingFolder, r.Arg<ObjectFileModel>().ToFileName())
                });

            DoCustomSetup();

            BextProcessor = new BextProcessor(BwfMetaEditPath, BaseProcessingFolder, ProcessRunner, XmlExporter, Observers, Verifier, ConformancePointDocumentFactory);
            await BextProcessor.EmbedBextMetadata(Instances, Metadata);
        }

        private ConformancePointDocument GetConformancePointDocument()
        {
            var exportCall = XmlExporter.ReceivedCalls().SingleOrDefault();
            if (exportCall != null)
            {
                return exportCall.GetArguments().First() as ConformancePointDocument;
            }

            return null;
        }

        public abstract class CommonBextProcessorAddMetadataTests : BextProcessorAddMetadataTests
        {
            [Test]
            public void ItShouldCallXmlExporterWithCorrectPath()
            {
                XmlExporter.Received().ExportToFile(Arg.Any<ConformancePointDocument>(), Path.Combine(BaseProcessingFolder, ExpectedProcessingFolder, "core.xml"), Encoding.ASCII);
            }

            [Test]
            public void ItShouldCallXmlExporterWithValidConformancePointDocument()
            {
                Assert.That(GetConformancePointDocument(), Is.Not.Null);
            }

            [Test]
            public void ConformancePointObjectShouldHaveCorrectNumberOfFiles()
            {
                Assert.That(GetConformancePointDocument().File.Count(), Is.EqualTo(Instances.Count));
            }

            [Test]
            public void ShouldCallProcessRunnerCorrectly()
            {
                var expectedArgs = string.Format("--verbose --Append --in-core={0}", Path.Combine(BaseProcessingFolder,ExpectedProcessingFolder, "core.xml").ToQuoted());
                ProcessRunner.Received().Run(Arg.Is<ProcessStartInfo>(i =>
                    i.Arguments.Equals(expectedArgs) &&
                    i.RedirectStandardError &&
                    i.RedirectStandardOutput &&
                    i.UseShellExecute == false &&
                    i.CreateNoWindow));
            }

            [Test]
            public void ShouldCallVerifierCorrectly()
            {
                Verifier.Received().Verify(Output.ToLowerInvariant(),
                    Arg.Is<List<string>>(l => l.Count() == Instances.Count));

                foreach (var instance in Instances)
                {
                    var path = Path.Combine(ExpectedProcessingFolder, instance.ToFileName()).ToLowerInvariant();
                    Verifier.Received().Verify(Output.ToLowerInvariant(), Arg.Is<List<string>>(l => l.Contains(path)));
                }
            }

            [Test]
            public void ConformancePointDocumentShouldIncludeExpectedFileEntries()
            {
                foreach (var file in Instances.Select(instance =>
                    GetConformancePointDocument().File.SingleOrDefault(
                        f => f.Name.Equals(Path.Combine(ExpectedProcessingFolder, instance.ToFileName())))))
                {
                    Assert.That(file, Is.Not.Null);
                }
            }
        }

        [TestFixture]
        public class WhenOnlyPresMasterProvenancePresent : CommonBextProcessorAddMetadataTests
        {
            protected override void DoCustomSetup()
            {
                Verifier.Verify(null, Arg.Any<List<string>>()).ReturnsForAnyArgs(true);

                Instances = new List<ObjectFileModel> {PresFileModel, ProdFileModel};
                Metadata.FileProvenances = new List<DigitalFileProvenance> {PreservationProvenance};
            }

            [Test]
            public void ItShouldUsePresProvenanceToGenerateAllCodingHistories()
            {
                ConformancePointDocumentFactory.Received().Generate(PresFileModel, PreservationProvenance, Metadata);
                ConformancePointDocumentFactory.Received().Generate(ProdFileModel, PreservationProvenance, Metadata);
            }
        }

        [TestFixture]
        public class WhenPresAndProdProvenancesPresent : CommonBextProcessorAddMetadataTests
        {
            protected override void DoCustomSetup()
            {
                Verifier.Verify(null, Arg.Any<List<string>>()).ReturnsForAnyArgs(true);

                Instances = new List<ObjectFileModel> {PresFileModel, ProdFileModel};
                Metadata.FileProvenances = new List<DigitalFileProvenance> {PreservationProvenance, ProductionProvenance};
            }

            [Test]
            public void ItShouldCallConformancePointDocumentFactoryCorrectly()
            {
                ConformancePointDocumentFactory.Received().Generate(PresFileModel, PreservationProvenance, Metadata);
                ConformancePointDocumentFactory.Received().Generate(ProdFileModel, ProductionProvenance, Metadata);
            }
        }

        [TestFixture]
        public class WhenPresIntAndPresProvenancesPresent : CommonBextProcessorAddMetadataTests
        {
            protected override void DoCustomSetup()
            {
                Verifier.Verify(null, Arg.Any<List<string>>()).ReturnsForAnyArgs(true);

                Instances = new List<ObjectFileModel> {PresFileModel, ProdFileModel, PresIntFileModel};
                Metadata.FileProvenances = new List<DigitalFileProvenance> {PreservationProvenance, PreservationIntermediateProvenance};
            }

            [Test]
            public void ItShouldCallConformancePointDocumentFactoryCorrectly()
            {
                ConformancePointDocumentFactory.Received().Generate(PresIntFileModel, PreservationIntermediateProvenance, Metadata);
                ConformancePointDocumentFactory.Received().Generate(ProdFileModel, PreservationIntermediateProvenance, Metadata);
                ConformancePointDocumentFactory.Received().Generate(PresFileModel, PreservationProvenance, Metadata);
            }
        }

        [TestFixture]
        public class WhenPresIntAndPresAndProdProvenancesPresent : CommonBextProcessorAddMetadataTests
        {
            protected override void DoCustomSetup()
            {
                Verifier.Verify(null, Arg.Any<List<string>>()).ReturnsForAnyArgs(true);

                Instances = new List<ObjectFileModel> {PresFileModel, ProdFileModel, PresIntFileModel};
                Metadata.FileProvenances = new List<DigitalFileProvenance> {PreservationProvenance, PreservationIntermediateProvenance, ProductionProvenance};
            }

            [Test]
            public void ItShouldCallConformancePointDocumentFactoryCorrectly()
            {
                ConformancePointDocumentFactory.Received().Generate(PresIntFileModel, PreservationIntermediateProvenance, Metadata);
                ConformancePointDocumentFactory.Received().Generate(PresFileModel, PreservationProvenance, Metadata);
                ConformancePointDocumentFactory.Received().Generate(ProdFileModel, ProductionProvenance, Metadata);
            }
        }

        [TestFixture]
        public class WhenMultipleSequenceInstancesPresent : CommonBextProcessorAddMetadataTests
        {
            private const string PreservationFileNameS2 = "MDPI_4890764553278906_02_pres.wav";
            private ObjectFileModel PresModelS2 { get; set; }
            private DigitalFileProvenance PresModelS2Provenance { get; set; }

            protected override void DoCustomSetup()
            {
                base.DoCustomSetup();
                PresModelS2 = new ObjectFileModel(PreservationFileNameS2);
                PresModelS2Provenance = new DigitalFileProvenance {Filename = PreservationFileNameS2};
                Instances = new List<ObjectFileModel> {PresFileModel, PresModelS2};
                Metadata.FileProvenances = new List<DigitalFileProvenance> {PreservationProvenance, PresModelS2Provenance};
            }

            [Test]
            public void ItShouldCallConformancePointDocumentFactoryCorrectly()
            {
                ConformancePointDocumentFactory.Received().Generate(PresFileModel, PreservationProvenance, Metadata);
                ConformancePointDocumentFactory.Received().Generate(PresModelS2, PresModelS2Provenance, Metadata);
            }
        }
    }
}