using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using NSubstitute;
using NSubstitute.Exceptions;
using NUnit.Framework;
using Packager.Models.FileModels;
using Packager.Models.OutputModels;
using Packager.Models.PodMetadataModels;
using Packager.Utilities;

namespace Packager.Test.Utilities
{
    [TestFixture]
    public class MetadataGeneratorTests
    {
        private const string PreservationFileName = "MDPI_4890764553278906_01_pres.wav";
        private const string ProductionFileName = "MDPI_4890764553278906_01_prod.wav";
        private const string AccessFileName = "MDPI_4890764553278906_01_access.mp4";


        private ObjectFileModel PreservationFileModel { get; set; }
        private ObjectFileModel ProductionFileModel { get; set; }
        private ObjectFileModel AccessFileModel { get; set; }

        private string ProcessingDirectory { get; set; }
        private List<ObjectFileModel> FilesToProcess { get; set; }
        private ConsolidatedPodMetadata PodMetadata { get; set; }
        private IHasher Hasher { get; set; }
        private CarrierData Result { get; set; }

        [SetUp]
        public void BeforeEach()
        {
            ProcessingDirectory = "test folder";

            PreservationFileModel = new ObjectFileModel(PreservationFileName);
            ProductionFileModel = new ObjectFileModel(ProductionFileName);
            AccessFileModel = new ObjectFileModel(AccessFileName);



            FilesToProcess = new List<ObjectFileModel> { PreservationFileModel, ProductionFileModel, AccessFileModel };

            PodMetadata = new ConsolidatedPodMetadata
            {
                Barcode = "11111111",
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

                FileProvenances = new List<DigitalFileProvenance>{new DigitalFileProvenance
                {
                    Filename = PreservationFileName, 
                    AdManufacturer = "Ad Manufacturer", 
                    AdModel = "Ad Model", 
                    AdSerialNumber = "Ad Serial Number", Comment = "File provenance comment", 
                    CreatedAt = new DateTime(2015, 05, 01).ToString(CultureInfo.InvariantCulture),
                    CreatedBy = "Test user",
                    DateDigitized = new DateTime(2015, 05, 01).ToString(CultureInfo.InvariantCulture), 
                    ExtractionWorkstation = "Extraction workstation", 
                    PlayerManufacturer = "Player manufacturer", 
                    PlayerModel = "Player model", 
                    PlayerSerialNumber = "Player serial number", SpeedUsed = "7.5 ips",
                    UpdatedAt = new DateTime(2015, 05, 01).ToString(CultureInfo.InvariantCulture)
                }}

            };

            Hasher = Substitute.For<IHasher>();
            Hasher.Hash(Path.Combine(ProcessingDirectory, PreservationFileName)).Returns("Preservation model hash value");
            Hasher.Hash(Path.Combine(ProcessingDirectory, ProductionFileName)).Returns("Production model hash value");
            Hasher.Hash(Path.Combine(ProcessingDirectory, AccessFileName)).Returns("Access model hash value");

            var generator = new MetadataGenerator(Hasher);
            Result = generator.GenerateMetadata(PodMetadata, FilesToProcess, ProcessingDirectory);
        }

        public class WhenSettingBasicProperties : MetadataGeneratorTests
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

        public class WhenSettingCleaningData : MetadataGeneratorTests
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

        public class WhenSettingBakingData : MetadataGeneratorTests
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

        public class WhenSettingConfigurationData : MetadataGeneratorTests
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

        public class WhenSettingPhysicalConditionData : MetadataGeneratorTests
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
        
        public class WhenSettingPartsData : MetadataGeneratorTests
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
        }
    }
}