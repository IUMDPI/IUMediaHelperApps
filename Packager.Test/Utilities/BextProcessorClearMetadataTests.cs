using System.Collections.Generic;
using NSubstitute;
using NUnit.Framework;
using Packager.Factories;
using Packager.Models.FileModels;
using Packager.Models.PodMetadataModels;
using Packager.Observers;
using Packager.Utilities.Bext;
using Packager.Utilities.ProcessRunners;
using Packager.Utilities.Xml;
using Packager.Verifiers;

namespace Packager.Test.Utilities
{
    [TestFixture]
    public class BextProcessorClearMetadataTests
    {
        [SetUp]
        public void BeforeEach()
        {
            Observers = Substitute.For<IObserverCollection>();
            ProdFileModel = FileModelFactory.GetModel(ProductionFileName);
            PresFileModel = FileModelFactory.GetModel(PreservationFileName);
            PresIntFileModel = FileModelFactory.GetModel(PreservationIntermediateFileName);

            Instances = new List<AbstractFile> {ProdFileModel, PresFileModel, PresIntFileModel};

            Verifier = Substitute.For<IBwfMetaEditResultsVerifier>();
            Verifier.Verify(Arg.Any<string>(), Arg.Any<string>()).Returns(true);

            MetaEditRunner = Substitute.For<IBwfMetaEditRunner>();
            /*MetaEditRunner.ClearMetadata(null).Returns(
                Task.FromResult((IProcessResult) new ProcessResult {ExitCode = 0, StandardError = Output, StandardOutput = Output}));
*/

            ConformancePointDocumentFactory = Substitute.For<IEmbeddedMetadataFactory<AudioPodMetadata>>();

            BextProcessor = new BextProcessor(MetaEditRunner, Observers, Verifier);
//            await BextProcessor.ClearAllBextMetadataFields(Instances);
        }

        private const string ProductionFileName = "MDPI_4890764553278906_01_prod.wav";
        private const string PreservationIntermediateFileName = "MDPI_4890764553278906_01_presInt.wav";
        private const string PreservationFileName = "MDPI_4890764553278906_01_pres.wav";

        private const string Output = "Output";
        private BextProcessor BextProcessor { get; set; }

        private IBwfMetaEditRunner MetaEditRunner { get; set; }
        private IXmlExporter XmlExporter { get; set; }
        private IBwfMetaEditResultsVerifier Verifier { get; set; }
        private IEmbeddedMetadataFactory<AudioPodMetadata> ConformancePointDocumentFactory { get; set; }
        private IObserverCollection Observers { get; set; }
        private AbstractFile ProdFileModel { get; set; }
        private AbstractFile PresFileModel { get; set; }
        private AbstractFile PresIntFileModel { get; set; }
        private List<AbstractFile> Instances { get; set; }
        private AudioPodMetadata Metadata { get; set; }

        [Test]
        public void ItShouldCallUtilityCorrectly()
        {
            foreach (var model in Instances)
            {
                //MetaEditRunner.Received().ClearMetadata(model);
            }
        }
    }
}