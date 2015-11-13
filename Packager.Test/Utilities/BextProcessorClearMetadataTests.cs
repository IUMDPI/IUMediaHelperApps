using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using NSubstitute;
using NUnit.Framework;
using Packager.Extensions;
using Packager.Factories;
using Packager.Models.FileModels;
using Packager.Models.PodMetadataModels;
using Packager.Models.ResultModels;
using Packager.Observers;
using Packager.Utilities;
using Packager.Verifiers;

namespace Packager.Test.Utilities
{
    [TestFixture]
    public class BextProcessorClearMetadataTests
    {
        [SetUp]
        public async void BeforeEach()
        {
            Observers = Substitute.For<IObserverCollection>();
            ProdFileModel = new ObjectFileModel(ProductionFileName);
            PresFileModel = new ObjectFileModel(PreservationFileName);
            PresIntFileModel = new ObjectFileModel(PreservationIntermediateFileName);

            Instances = new List<ObjectFileModel> {ProdFileModel, PresFileModel, PresIntFileModel};

            Verifier = Substitute.For<IBwfMetaEditResultsVerifier>();
            Verifier.Verify(Arg.Any<string>(), Arg.Any<string>()).Returns(true);

            MetaEditRunner = Substitute.For<IBwfMetaEditRunner>();
            /*MetaEditRunner.ClearMetadata(null).Returns(
                Task.FromResult((IProcessResult) new ProcessResult {ExitCode = 0, StandardError = Output, StandardOutput = Output}));
*/

            ConformancePointDocumentFactory = Substitute.For<IBextMetadataFactory>();

            BextProcessor = new BextProcessor(MetaEditRunner, Observers, Verifier);
//            await BextProcessor.ClearAllBextMetadataFields(Instances);
        }
        
        private const string ProductionFileName = "MDPI_4890764553278906_01_prod.wav";
        private const string PreservationIntermediateFileName = "MDPI_4890764553278906_01_pres-int.wav";
        private const string PreservationFileName = "MDPI_4890764553278906_01_pres.wav";
   
        private const string Output = "Output";
        private BextProcessor BextProcessor { get; set; }
       
        private IBwfMetaEditRunner MetaEditRunner { get; set; }
        private IXmlExporter XmlExporter { get; set; }
        private IBwfMetaEditResultsVerifier Verifier { get; set; }
        private IBextMetadataFactory ConformancePointDocumentFactory { get; set; }
        private IObserverCollection Observers { get; set; }
        private ObjectFileModel ProdFileModel { get; set; }
        private ObjectFileModel PresFileModel { get; set; }
        private ObjectFileModel PresIntFileModel { get; set; }
        private List<ObjectFileModel> Instances { get; set; }
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