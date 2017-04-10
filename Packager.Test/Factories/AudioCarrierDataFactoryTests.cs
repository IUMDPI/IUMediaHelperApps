using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using NSubstitute;
using NUnit.Framework;
using Packager.Factories;
using Packager.Models.FileModels;
using Packager.Models.OutputModels.Carrier;
using Packager.Models.PodMetadataModels;

namespace Packager.Test.Factories
{
    [TestFixture]
    public class AudioCarrierDataFactoryTests
    {
        protected virtual void SetUpMetadata()
        {
            PodMetadata = new AudioPodMetadata
            {
                Barcode = "4890764553278906",
                Brand = "Brand",
                BakingDate = new DateTime(2015, 05, 01),
                CleaningDate = new DateTime(2015, 05, 01),
                CleaningComment = "Cleaning comment",
                Format = "CD-R",
                DirectionsRecorded = "1",
                CallNumber = "Call number",
                SoundField = "Mono",
                PlaybackSpeed = "7.5 ips",
                TapeThickness = "1 mm",
                Repaired = "Yes",
                Comments = "comments value"
            };
        }
        [SetUp]
        public async Task BeforeEach()
        {
            ProcessingDirectory = "test folder";

            PreservationSide1FileModel = FileModelFactory.GetModel(PreservationSide1FileName);
            ProductionSide1FileModel = FileModelFactory.GetModel(ProductionSide1FileName);
            AccessSide1FileModel = FileModelFactory.GetModel(AccessSide1FileName);

            SetUpMetadata();

            SideDataFactory = Substitute.For<ISideDataFactory>();

            FilesToProcess = new List<AbstractFile>
            {
                PreservationSide1FileModel,
                ProductionSide1FileModel,
                AccessSide1FileModel
            };

            var generator = new AudioCarrierDataFactory(SideDataFactory);
            Result = await generator.Generate(PodMetadata, DigitizingEntity, FilesToProcess, CancellationToken.None) as AudioCarrier;
        }

        private const string PreservationSide1FileName = "MDPI_4890764553278906_01_pres.wav";
        private const string ProductionSide1FileName = "MDPI_4890764553278906_01_prod.wav";
        private const string AccessSide1FileName = "MDPI_4890764553278906_01_access.mp4";
        private const string DigitizingEntity = "Test digitizing entity";

        private AbstractFile PreservationSide1FileModel { get; set; }
        private AbstractFile ProductionSide1FileModel { get; set; }
        private AbstractFile AccessSide1FileModel { get; set; }
        private string ProcessingDirectory { get; set; }
        private List<AbstractFile> FilesToProcess { get; set; }
        private AudioPodMetadata PodMetadata { get; set; }
        private ISideDataFactory SideDataFactory { get; set; }
        private AudioCarrier Result { get; set; }

        public class WhenSettingBasicProperties : AudioCarrierDataFactoryTests
        {
            public class WhenCallNumberNotSet : WhenSettingBasicProperties
            {
                protected override void SetUpMetadata()
                {
                    base.SetUpMetadata();
                    PodMetadata.CallNumber = "";
                }

                [Test]
                public void ItShouldSetIdentifierCorrectly()
                {
                    Assert.That(Result.Identifier, Is.EqualTo("Unknown"));
                }

            }

            public class WhenCallNumberSet : WhenSettingBasicProperties
            {
                [Test]
                public void ItShouldSetIdentifierCorrectly()
                {
                    Assert.That(Result.Identifier, Is.EqualTo(PodMetadata.CallNumber));
                }

            }

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
            public void ItShouldSetDirectionsRecordedCorrectly()
            {
                Assert.That(Result.DirectionsRecorded, Is.EqualTo(PodMetadata.DirectionsRecorded));
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

            [Test]
            public void ItShouldSetCommentsCorrectly()
            {
                Assert.That(Result.Comments, Is.EqualTo("comments value"));
            }
        }

        public class WhenSettingCleaningData : AudioCarrierDataFactoryTests
        {
            [Test]
            public void ItShouldSetCleaningCommentCorrectly()
            {
                Assert.That(Result.Cleaning.Comment, Is.EqualTo(PodMetadata.CleaningComment));
            }

            [Test]
            public void ItShouldSetCleaningDateCorrectly()
            {
                Assert.That(Result.Cleaning.Date, Is.EqualTo(PodMetadata.CleaningDate?.ToString("yyyy-MM-dd")));
            }

            [Test]
            public void ItShouldSetCleaningObject()
            {
                Assert.That(Result.Cleaning, Is.Not.Null);
            }
        }

        public class WhenSettingBakingData : AudioCarrierDataFactoryTests
        {
            [Test]
            public void ItShouldSetBakingDateCorrectly()
            {
                Assert.That(Result.Baking.Date, Is.EqualTo(PodMetadata.BakingDate?.ToString("yyyy-MM-dd")));
            }

            [Test]
            public void ItShouldSetBakingObject()
            {
                Assert.That(Result.Baking, Is.Not.Null);
            }
        }

        public class WhenSettingConfigurationData : AudioCarrierDataFactoryTests
        {
            [Test]
            public void ItShouldSetConfigurationObject()
            {
                Assert.That(Result.Configuration, Is.Not.Null);
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

            [Test]
            public void ItShouldSetTrackCorrectly()
            {
                Assert.That(Result.Configuration.Track, Is.EqualTo(PodMetadata.TrackConfiguration));
            }
        }

        public class WhenSettingPhysicalConditionData : AudioCarrierDataFactoryTests
        {
            [Test]
            public void ItShouldSetDamageCorrectly()
            {
                Assert.That(Result.PhysicalCondition.Damage, Is.EqualTo(PodMetadata.Damage));
            }

            [Test]
            public void ItShouldSetPhysicalConditionObject()
            {
                Assert.That(Result.PhysicalCondition, Is.Not.Null);
            }

            [Test]
            public void ItShouldSetPreservationProblemCorrectly()
            {
                Assert.That(Result.PhysicalCondition.PreservationProblem, Is.EqualTo(PodMetadata.PreservationProblems));
            }
        }

        public class WhenSettingPartsData : AudioCarrierDataFactoryTests
        {
            [Test]
            public void ItShouldCallSideDataFactoryCorrectly()
            {
                SideDataFactory.Received().Generate(PodMetadata, FilesToProcess);
            }

            [Test]
            public void ItShouldSetDigitizingEntityCorrectly()
            {
                Assert.That(Result.Parts.DigitizingEntity, Is.EqualTo(DigitizingEntity));
            }

            [Test]
            public void ItShouldSetPartsDataObjectCorrectly()
            {
                Assert.That(Result.Parts, Is.Not.Null);
            }
        }
    }
}