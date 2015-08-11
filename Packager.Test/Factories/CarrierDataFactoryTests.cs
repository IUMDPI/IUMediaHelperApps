using System;
using System.Collections.Generic;
using System.Globalization;
using NSubstitute;
using NUnit.Framework;
using Packager.Factories;
using Packager.Models.FileModels;
using Packager.Models.OutputModels;
using Packager.Models.PodMetadataModels;

namespace Packager.Test.Factories
{
    [TestFixture]
    public class CarrierDataFactoryTests
    {
        private const string PreservationSide1FileName = "MDPI_4890764553278906_01_pres.wav";
        private const string ProductionSide1FileName = "MDPI_4890764553278906_01_prod.wav";
        private const string AccessSide1FileName = "MDPI_4890764553278906_01_access.mp4";

        private ObjectFileModel PreservationSide1FileModel { get; set; }
        private ObjectFileModel ProductionSide1FileModel { get; set; }
        private ObjectFileModel AccessSide1FileModel { get; set; }

       
        private string ProcessingDirectory { get; set; }
        private List<ObjectFileModel> FilesToProcess { get; set; }
        private ConsolidatedPodMetadata PodMetadata { get; set; }
        private ISideDataFactory SideDataFactory { get; set; }
        private CarrierData Result { get; set; }

       

        [SetUp]
        public void BeforeEach()
        {
            ProcessingDirectory = "test folder";

            PreservationSide1FileModel = new ObjectFileModel(PreservationSide1FileName);
            ProductionSide1FileModel = new ObjectFileModel(ProductionSide1FileName);
            AccessSide1FileModel = new ObjectFileModel(AccessSide1FileName);
            
            PodMetadata = new ConsolidatedPodMetadata
            {
                Barcode = "4890764553278906",
                Brand = "Brand",
                BakingDate = new DateTime(2015, 05, 01).ToString(CultureInfo.InvariantCulture),
                CleaningDate = new DateTime(2015, 05, 01).ToString(CultureInfo.InvariantCulture),
                CleaningComment = "Cleaning comment",
                Format = "CD-R",
                DirectionsRecorded = "1",
                CallNumber = "Call number",
                DigitizingEntity = "Test entity",
                SoundField = "Mono",
                PlaybackSpeed = "7.5 ips",
                Identifier = "1",
                TapeThickness = "1 mm",
                Repaired = "Yes",
            };
            
            SideDataFactory = Substitute.For<ISideDataFactory>();

            FilesToProcess = new List<ObjectFileModel> { PreservationSide1FileModel, ProductionSide1FileModel, AccessSide1FileModel };
            
            var generator = new CarrierDataFactory(SideDataFactory);
            Result = generator.Generate(PodMetadata, FilesToProcess, ProcessingDirectory);
        }

        public class WhenSettingBasicProperties : CarrierDataFactoryTests
        {
            [Test]
            public void ItShouldSetBarCodeCorrectly()
            {
                Assert.That(Result.Barcode, Is.EqualTo(PodMetadata.Barcode));
            }

            [Test]
            public void ItShouldSetBrandCorrectly()
            {
                Assert.That(Result.Brand, Is.EqualTo(PodMetadata.Brand));
            }

            [Test]
            public void ItShouldSetCarrierTypeCorrectly()
            {
                Assert.That(Result.CarrierType, Is.EqualTo(PodMetadata.Format));
            }

            [Test]
            public void ItShouldSetXsiTypeCorrectly()
            {
                Assert.That(Result.XsiType, Is.EqualTo(string.Format("{0}Carrier", PodMetadata.Format)));
            }

            [Test]
            public void ItShouldSetDirectionsRecordedCorrectly()
            {
                Assert.That(Result.DirectionsRecorded, Is.EqualTo(PodMetadata.DirectionsRecorded));
            }

            [Test]
            public void ItShouldSetIdentifierCorrectly()
            {
                Assert.That(Result.Identifier, Is.EqualTo(PodMetadata.Identifier));
            }

            [Test]
            public void ItShouldSetThicknessCorrectly()
            {
                Assert.That(Result.Thickness, Is.EqualTo(PodMetadata.TapeThickness));
            }

            [Test]
            public void ItShouldSetRepairedCorrectly()
            {
                Assert.That(Result.Repaired, Is.EqualTo(PodMetadata.Repaired));
            }
        }

        public class WhenSettingCleaningData : CarrierDataFactoryTests
        {
            [Test]
            public void ItShouldSetCleaningObject()
            {
                Assert.That(Result.Cleaning, Is.Not.Null);
            }

            [Test]
            public void ItShouldSetCleaningDateCorrectly()
            {
                Assert.That(Result.Cleaning.Date, Is.EqualTo(PodMetadata.CleaningDate));
            }

            [Test]
            public void ItShouldSetCleaningCommentCorrectly()
            {
                Assert.That(Result.Cleaning.Comment, Is.EqualTo(PodMetadata.CleaningComment));
            }
        }

        public class WhenSettingBakingData : CarrierDataFactoryTests
        {
            [Test]
            public void ItShouldSetBakingObject()
            {
                Assert.That(Result.Baking, Is.Not.Null);
            }

            [Test]
            public void ItShouldSetBakingDateCorrectly()
            {
                Assert.That(Result.Baking.Date, Is.EqualTo(PodMetadata.BakingDate));
            }
        }

        public class WhenSettingConfigurationData : CarrierDataFactoryTests
        {
            [Test]
            public void ItShouldSetConfigurationObject()
            {
                Assert.That(Result.Configuration, Is.Not.Null);
            }

            [Test]
            public void ItShouldSetXsiTypeCorrectly()
            {
                Assert.That(Result.Configuration.XsiType, Is.EqualTo(string.Format("Configuration{0}", PodMetadata.Format)));
            }

            [Test]
            public void ItShouldSetTrackCorrectly()
            {
                Assert.That(Result.Configuration.Track, Is.EqualTo(PodMetadata.TrackConfiguration));
            }

            [Test]
            public void ItShouldSetSoundFieldCorrectly()
            {
                Assert.That(Result.Configuration.SoundField, Is.EqualTo(PodMetadata.SoundField));
            }

            [Test]
            public void ItShouldSetSpeedCorrectly()
            {
                Assert.That(Result.Configuration.Speed, Is.EqualTo(PodMetadata.PlaybackSpeed));
            }
        }

        public class WhenSettingPhysicalConditionData : CarrierDataFactoryTests
        {
            [Test]
            public void ItShouldSetPhysicalConditionObject()
            {
                Assert.That(Result.PhysicalCondition, Is.Not.Null);
            }

            [Test]
            public void ItShouldSetDamageCorrectly()
            {
                Assert.That(Result.PhysicalCondition.Damage, Is.EqualTo(PodMetadata.Damage));
            }

            [Test]
            public void ItShouldSetPreservationProblemCorrectly()
            {
                Assert.That(Result.PhysicalCondition.PreservationProblem, Is.EqualTo(PodMetadata.PreservationProblems));
            }
        }

        public class WhenSettingPartsData : CarrierDataFactoryTests
        {
            [Test]
            public void ItShouldSetPartsDataObjectCorrectly()
            {
                Assert.That(Result.Parts, Is.Not.Null);
            }

            [Test]
            public void ItShouldSetDigitizingEntityCorrectly()
            {
                Assert.That(Result.Parts.DigitizingEntity, Is.EqualTo(PodMetadata.DigitizingEntity));
            }

            [Test]
            public void ItShouldCallSideDataFactoryCorrectly()
            {
                SideDataFactory.Received().Generate(PodMetadata, FilesToProcess, ProcessingDirectory);
            }
        }
    }
}