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
    public class VideoSideDataFactoryTests
    {
        [SetUp]
        public void BeforeEach()
        {
            ProcessingDirectory = "test folder";

            PreservationSide1FileModel = FileModelFactory.GetModel(PreservationSide1FileName);
            PreservationSide1FileModel.Checksum = "pres 1 hash";

            ProductionSide1FileModel = FileModelFactory.GetModel(MezzanineSide1FileName);
            ProductionSide1FileModel.Checksum = "prod 1 hash";

            AccessSide1FileModel = FileModelFactory.GetModel(AccessSide1FileName);
            AccessSide1FileModel.Checksum = "access 1 hash";

            PreservationSide2FileModel = FileModelFactory.GetModel(PreservationSide2FileName);
            PreservationSide2FileModel.Checksum = "pres 2 hash";

            ProductionSide2FileModel = FileModelFactory.GetModel(MezzanineSide2FileName);
            ProductionSide2FileModel.Checksum = "prod 2 hash";

            AccessSide2FileModel = FileModelFactory.GetModel(AccessSide2FileName);
            AccessSide2FileModel.Checksum = "access 2 hash";

            PreservationIntermediateSide1FileModel = FileModelFactory.GetModel(PreservationIntermediateSide1FileName);
            PreservationIntermediateSide1FileModel.Checksum = "presInt 1 hash";

            IngestDataFactory = Substitute.For<IIngestDataFactory>();
            IngestDataFactory.Generate((VideoPodMetadata)null, null).ReturnsForAnyArgs(new VideoIngest());

            PodMetadata = new VideoPodMetadata();

            DoCustomSetup();

            var factory = new SideDataFactory(IngestDataFactory);
            Results = factory.Generate(PodMetadata, FilesToProcess);
        }

        private const string PreservationIntermediateSide1FileName = "MDPI_4890764553278906_01_presInt.mkv";
        private const string PreservationSide1FileName = "MDPI_4890764553278906_01_pres.mkv";
        private const string MezzanineSide1FileName = "MDPI_4890764553278906_01_mezz.mov";
        private const string AccessSide1FileName = "MDPI_4890764553278906_01_access.mp4";
        private const string PreservationSide2FileName = "MDPI_4890764553278906_02_pres.wav";
        private const string MezzanineSide2FileName = "MDPI_4890764553278906_02_mezz.mov";
        private const string AccessSide2FileName = "MDPI_4890764553278906_02_access.mp4";
        private int ExpectedSides { get; set; }
        private AbstractFile ExpectedSide1MasterFileModel { get; set; }
        private AbstractFile ExpectedSide2MasterFileModel { get; set; }
        private AbstractFile PreservationIntermediateSide1FileModel { get; set; }

        private AbstractFile PreservationSide1FileModel { get; set; }
        private AbstractFile ProductionSide1FileModel { get; set; }
        private AbstractFile AccessSide1FileModel { get; set; }
        private AbstractFile PreservationSide2FileModel { get; set; }
        private AbstractFile ProductionSide2FileModel { get; set; }
        private AbstractFile AccessSide2FileModel { get; set; }
        private List<AbstractFile> FilesToProcess { get; set; }

        private IIngestDataFactory IngestDataFactory { get; set; }
        private VideoPodMetadata PodMetadata { get; set; }
        private string ProcessingDirectory { get; set; }
        private SideData[] Results { get; set; }

        protected virtual void DoCustomSetup()
        {
            FilesToProcess = new List<AbstractFile> {PreservationSide1FileModel, ProductionSide1FileModel, AccessSide1FileModel};
            ExpectedSide1MasterFileModel = PreservationSide1FileModel;
            ExpectedSides = 1;
        }

        public class WhenPreservationIntermediateMasterPresent : VideoSideDataFactoryTests
        {
            protected override void DoCustomSetup()
            {
                base.DoCustomSetup();
                FilesToProcess.Add(PreservationIntermediateSide1FileModel);
                ExpectedSide1MasterFileModel = PreservationIntermediateSide1FileModel;
            }
        }

        public class WhenMoreThanOneAudioSidePresent : VideoSideDataFactoryTests
        {
            protected override void DoCustomSetup()
            {
                base.DoCustomSetup();
                FilesToProcess = new List<AbstractFile>
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
        public void QCStatusShouldBeSetCorrect()
        {
            foreach (var side in Results)
            {
                Assert.That(side.QCStatus, Is.EqualTo("OK"));
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