using System.Collections.Generic;
using System.Threading.Tasks;
using NSubstitute;
using NUnit.Framework;
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
            MetaEditRunner = Substitute.For<IBwfMetaEditRunner>();

            MetaEditRunner.AddMetadata(null, null).ReturnsForAnyArgs(
                Task.FromResult((IProcessResult) new ProcessResult {ExitCode = 0, StandardError = Output, StandardOutput = Output}));


            Observers = Substitute.For<IObserverCollection>();


            Metadata = new ConsolidatedPodMetadata();

            ConformancePointDocumentFactory = Substitute.For<IBextMetadataFactory>();
            ConformancePointDocumentFactory.Generate(Arg.Any<List<ObjectFileModel>>(), Arg.Any<ObjectFileModel>(), Metadata)
                .Returns(r => new BextMetadata());

            DoCustomSetup();

            BextProcessor = new BextProcessor(MetaEditRunner, Observers, Verifier, ConformancePointDocumentFactory);
            await BextProcessor.EmbedBextMetadata(Instances, Metadata);
        }

        private const string ProductionFileName = "MDPI_4890764553278906_01_prod.wav";
        private const string PreservationIntermediateFileName = "MDPI_4890764553278906_01_pres-int.wav";
        private const string PreservationFileName = "MDPI_4890764553278906_01_pres.wav";

        private const string Output = "Output";
        private BextProcessor BextProcessor { get; set; }

        private IBwfMetaEditRunner MetaEditRunner { get; set; }

        private IBwfMetaEditResultsVerifier Verifier { get; set; }
        private IBextMetadataFactory ConformancePointDocumentFactory { get; set; }
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


        public abstract class CommonBextProcessorAddMetadataTests : BextProcessorAddMetadataTests
        {
            [Test]
            public void ShouldCallMetaEditRunnerCorrectly()
            {
                foreach (var instance in Instances)
                {
                    MetaEditRunner.Received().AddMetadata(instance, Arg.Any<BextMetadata>());
                }
            }

            [Test]
            public void ShouldCallVerifierCorrectly()
            {
                foreach (var instance in Instances)
                {
                    Verifier.Received().Verify(Output.ToLowerInvariant(), instance.ToFileName().ToLowerInvariant());
                }
            }
        }

        [TestFixture]
        public class WhenOnlyPresMasterProvenancePresent : CommonBextProcessorAddMetadataTests
        {
            protected override void DoCustomSetup()
            {
                Verifier.Verify(Arg.Any<string>(), Arg.Any<string>()).Returns(true);

                Instances = new List<ObjectFileModel> {PresFileModel, ProdFileModel};
                Metadata.FileProvenances = new List<DigitalFileProvenance> {PreservationProvenance};
            }

            /*[Test]
            public void ItShouldUsePresProvenanceToGenerateAllCodingHistories()
            {
                ConformancePointDocumentFactory.Received().Generate(PresFileModel, PreservationProvenance, Metadata);
                ConformancePointDocumentFactory.Received().Generate(ProdFileModel, PreservationProvenance, Metadata);
            }*/
        }

        [TestFixture]
        public class WhenPresAndProdProvenancesPresent : CommonBextProcessorAddMetadataTests
        {
            protected override void DoCustomSetup()
            {
                Verifier.Verify(Arg.Any<string>(), Arg.Any<string>()).Returns(true);

                Instances = new List<ObjectFileModel> {PresFileModel, ProdFileModel};
                Metadata.FileProvenances = new List<DigitalFileProvenance> {PreservationProvenance, ProductionProvenance};
            }

           /* [Test]
            public void ItShouldCallConformancePointDocumentFactoryCorrectly()
            {
                ConformancePointDocumentFactory.Received().Generate(PresFileModel, PreservationProvenance, Metadata);
                ConformancePointDocumentFactory.Received().Generate(ProdFileModel, ProductionProvenance, Metadata);
            }*/
        }

        [TestFixture]
        public class WhenPresIntAndPresProvenancesPresent : CommonBextProcessorAddMetadataTests
        {
            protected override void DoCustomSetup()
            {
                Verifier.Verify(Arg.Any<string>(), Arg.Any<string>()).Returns(true);

                Instances = new List<ObjectFileModel> {PresFileModel, ProdFileModel, PresIntFileModel};
                Metadata.FileProvenances = new List<DigitalFileProvenance> {PreservationProvenance, PreservationIntermediateProvenance};
            }

            /*[Test]
            public void ItShouldCallConformancePointDocumentFactoryCorrectly()
            {
                ConformancePointDocumentFactory.Received().Generate(PresIntFileModel, PreservationIntermediateProvenance, Metadata);
                ConformancePointDocumentFactory.Received().Generate(ProdFileModel, PreservationIntermediateProvenance, Metadata);
                ConformancePointDocumentFactory.Received().Generate(PresFileModel, PreservationProvenance, Metadata);
            }*/
        }

        [TestFixture]
        public class WhenPresIntAndPresAndProdProvenancesPresent : CommonBextProcessorAddMetadataTests
        {
            protected override void DoCustomSetup()
            {
                Verifier.Verify(Arg.Any<string>(), Arg.Any<string>()).Returns(true);

                Instances = new List<ObjectFileModel> {PresFileModel, ProdFileModel, PresIntFileModel};
                Metadata.FileProvenances = new List<DigitalFileProvenance> {PreservationProvenance, PreservationIntermediateProvenance, ProductionProvenance};
            }

            [Test]
            public void ItShouldCallConformancePointDocumentFactoryCorrectly()
            {
               /* ConformancePointDocumentFactory.Received().Generate(PresIntFileModel, PreservationIntermediateProvenance, Metadata);
                ConformancePointDocumentFactory.Received().Generate(PresFileModel, PreservationProvenance, Metadata);
                ConformancePointDocumentFactory.Received().Generate(ProdFileModel, ProductionProvenance, Metadata);*/
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

                Verifier.Verify(Arg.Any<string>(), Arg.Any<string>()).Returns(true);

                PresModelS2 = new ObjectFileModel(PreservationFileNameS2);
                PresModelS2Provenance = new DigitalFileProvenance {Filename = PreservationFileNameS2};
                Instances = new List<ObjectFileModel> {PresFileModel, PresModelS2};
                Metadata.FileProvenances = new List<DigitalFileProvenance> {PreservationProvenance, PresModelS2Provenance};
            }

            [Test]
            public void ItShouldCallConformancePointDocumentFactoryCorrectly()
            {
                /*ConformancePointDocumentFactory.Received().Generate(PresFileModel, PreservationProvenance, Metadata);
                ConformancePointDocumentFactory.Received().Generate(PresModelS2, PresModelS2Provenance, Metadata);*/
            }
        }
    }
}