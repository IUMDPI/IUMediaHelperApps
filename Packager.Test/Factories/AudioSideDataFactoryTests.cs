using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using NSubstitute;
using NUnit.Framework;
using Packager.Factories;
using Packager.Models.FileModels;
using Packager.Models.OutputModels;
using Packager.Models.OutputModels.Ingest;
using Packager.Models.PodMetadataModels;

namespace Packager.Test.Factories
{
    [TestFixture]
    public class AudioSideDataFactoryTests
    {
        [SetUp]
        public void BeforeEach()
        {
            ProcessingDirectory = "test folder";

            PreservationSide1FileModel = new ObjectFileModel(PreservationSide1FileName) {Checksum = "pres 1 hash"};
            ProductionSide1FileModel = new ObjectFileModel(ProductionSide1FileName) { Checksum = "prod 1 hash" }; 
            AccessSide1FileModel = new ObjectFileModel(AccessSide1FileName) { Checksum = "access 1 hash" }; 
            PreservationSide2FileModel = new ObjectFileModel(PreservationSide2FileName) { Checksum = "pres 2 hash" }; 
            ProductionSide2FileModel = new ObjectFileModel(ProductionSide2FileName) { Checksum = "prod 2 hash" }; 
            AccessSide2FileModel = new ObjectFileModel(AccessSide2FileName) { Checksum = "access 1 hash" }; 

            PreservationIntermediateSide1FileModel = new ObjectFileModel(PreservationIntermediateSide1FileName) { Checksum = "presInt 1 hash" }; ;

            IngestDataFactory = Substitute.For<IIngestDataFactory>();
            IngestDataFactory.Generate((AudioPodMetadata)null, null).ReturnsForAnyArgs(new AudioIngest());

            PodMetadata = new AudioPodMetadata();

            DoCustomSetup();

            var factory = new SideDataFactory(IngestDataFactory);
            Results = factory.Generate(PodMetadata, FilesToProcess);
        }

        private const string PreservationIntermediateSide1FileName = "MDPI_4890764553278906_01_presInt.wav";
        private const string PreservationSide1FileName = "MDPI_4890764553278906_01_pres.wav";
        private const string ProductionSide1FileName = "MDPI_4890764553278906_01_prod.wav";
        private const string AccessSide1FileName = "MDPI_4890764553278906_01_access.mp4";
        private const string PreservationSide2FileName = "MDPI_4890764553278906_02_pres.wav";
        private const string ProductionSide2FileName = "MDPI_4890764553278906_02_prod.wav";
        private const string AccessSide2FileName = "MDPI_4890764553278906_02_access.mp4";
        private int ExpectedSides { get; set; }
        private ObjectFileModel ExpectedSide1MasterFileModel { get; set; }
        private ObjectFileModel ExpectedSide2MasterFileModel { get; set; }
        private ObjectFileModel PreservationIntermediateSide1FileModel { get; set; }

        private ObjectFileModel PreservationSide1FileModel { get; set; }
        private ObjectFileModel ProductionSide1FileModel { get; set; }
        private ObjectFileModel AccessSide1FileModel { get; set; }
        private ObjectFileModel PreservationSide2FileModel { get; set; }
        private ObjectFileModel ProductionSide2FileModel { get; set; }
        private ObjectFileModel AccessSide2FileModel { get; set; }
        private List<ObjectFileModel> FilesToProcess { get; set; }

        private IIngestDataFactory IngestDataFactory { get; set; }
        private AudioPodMetadata PodMetadata { get; set; }
        private string ProcessingDirectory { get; set; }
        private SideData[] Results { get; set; }

        protected virtual void DoCustomSetup()
        {
            FilesToProcess = new List<ObjectFileModel> {PreservationSide1FileModel, ProductionSide1FileModel, AccessSide1FileModel};
            ExpectedSide1MasterFileModel = PreservationSide1FileModel;
            ExpectedSides = 1;
        }

        public class WhenPreservationIntermediateMasterPresent : AudioSideDataFactoryTests
        {
            protected override void DoCustomSetup()
            {
                base.DoCustomSetup();
                FilesToProcess.Add(PreservationIntermediateSide1FileModel);
                ExpectedSide1MasterFileModel = PreservationIntermediateSide1FileModel;
            }
        }

        public class WhenMoreThanOneAudioSidePresent : AudioSideDataFactoryTests
        {
            protected override void DoCustomSetup()
            {
                base.DoCustomSetup();
                FilesToProcess = new List<ObjectFileModel>
                {
                    ProductionSide1FileModel,
                    ProductionSide2FileModel,
                    PreservationSide1FileModel,
                    PreservationSide2FileModel,
                    AccessSide1FileModel,
                    AccessSide2FileModel
                };
                ExpectedSide1MasterFileModel = PreservationSide1FileModel;
                ExpectedSide2MasterFileModel = PreservationSide2FileModel;
                ExpectedSides = 2;
            }

            [Test]
            public void ItShouldCallIngestFactoryCorrectlyForSide2()
            {
                IngestDataFactory.Received(1).Generate(PodMetadata, ExpectedSide2MasterFileModel);
            }
        }

        [Test]
        public void EachSideShouldHaveCorrectFilesEntries()
        {
            for (var i = 0; i < Results.Length; i++)
            {
                var side = Results[i];
                var modelsForSide = FilesToProcess.Where(m => m.SequenceIndicator.Equals(i + 1)).ToList();

                Assert.That(side.Files.Count, Is.EqualTo(modelsForSide.Count()));

                foreach (var model in modelsForSide)
                {
                    var fileData = side.Files.SingleOrDefault(f => f.FileName.Equals(model.ToFileName()));
                    Assert.That(fileData, Is.Not.Null);
                    Assert.That(string.IsNullOrWhiteSpace(fileData.Checksum), Is.False);
                }
            }
        }

        [Test]
        public void ItShouldCallIngestFactoryCorrectlyForSide1()
        {
            IngestDataFactory.Received(1).Generate(PodMetadata, ExpectedSide1MasterFileModel);
        }

        [Test]
        public void ManualCheckShouldBeSetCorrectly()
        {
            foreach (var side in Results)
            {
                Assert.That(side.ManualCheck, Is.EqualTo("No"));
            }
        }

        [Test]
        public void ResultShouldContainExpectedSides()
        {
            Assert.That(Results.Length, Is.EqualTo(ExpectedSides));
        }

        [Test]
        public void SideValuesShouldBeSetCorrectly()
        {
            for (var i = 0; i < Results.Length; i++)
            {
                Assert.That(Results[i].Side, Is.EqualTo((i + 1).ToString(CultureInfo.InvariantCulture)));
            }
        }
    }
}