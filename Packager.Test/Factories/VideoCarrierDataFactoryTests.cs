using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Common.Models;
using NSubstitute;
using NUnit.Framework;
using Packager.Factories;
using Packager.Models.FileModels;
using Packager.Models.OutputModels.Carrier;
using Packager.Models.PodMetadataModels;
using Packager.Providers;

namespace Packager.Test.Factories
{
    [TestFixture]
    public class VideoCarrierDataFactoryTests
    {
        protected virtual void SetupMetadata()
        {
            PodMetadata = new VideoPodMetadata
            {
                Barcode = "4890764553278906",
                BakingDate = new DateTime(2015, 05, 01),
                CleaningDate = new DateTime(2015, 05, 01),
                CleaningComment = "Cleaning comment",
                Format = MediaFormats.Cdr,
                CallNumber = "Call number",
                Repaired = "Yes",
                RecordingStandard = "recording standard",
                ImageFormat = "image format",
                Comments = "comments value"
            };

        }

        protected virtual void SetupMediaInfoProvider()
        {

            MediaInfoProvider = Substitute.For<IMediaInfoProvider>();
            MediaInfoProvider.GetMediaInfo(PreservationSide1FileModel, Arg.Any<CancellationToken>())
                .Returns(Task.FromResult(new MediaInfo()));

        }

        [SetUp]
        public async Task BeforeEach()
        {
            ProcessingDirectory = "test folder";

            PreservationSide1FileModel = FileModelFactory.GetModel(PreservationSide1FileName);
            MezzSide1FileModel = FileModelFactory.GetModel(MezzSide1FileName);
            AccessSide1FileModel = FileModelFactory.GetModel(AccessSide1FileName);

            SetupMetadata();
            SetupMediaInfoProvider();
           
            SideDataFactory = Substitute.For<ISideDataFactory>();

            FilesToProcess = new List<AbstractFile>
            {
                PreservationSide1FileModel,
                MezzSide1FileModel,
                AccessSide1FileModel
            };

            var generator = new VideoCarrierDataFactory(SideDataFactory, MediaInfoProvider);
            Result = await generator.Generate(PodMetadata, DigitizingEntity, FilesToProcess, CancellationToken.None) as VideoCarrier;
        }

        private const string PreservationSide1FileName = "MDPI_4890764553278906_01_pres.mkv";
        private const string MezzSide1FileName = "MDPI_4890764553278906_01_mezz.mov";
        private const string AccessSide1FileName = "MDPI_4890764553278906_01_access.mp4";
        private const string DigitizingEntity = "Test digitizing entity";

        private AbstractFile PreservationSide1FileModel { get; set; }
        private AbstractFile MezzSide1FileModel { get; set; }
        private AbstractFile AccessSide1FileModel { get; set; }
        
        private string ProcessingDirectory { get; set; }
        private List<AbstractFile> FilesToProcess { get; set; }
        private VideoPodMetadata PodMetadata { get; set; }
        private ISideDataFactory SideDataFactory { get; set; }
        private VideoCarrier Result { get; set; }

        private IMediaInfoProvider MediaInfoProvider { get; set; }

        public class WhenSettingBasicProperties : VideoCarrierDataFactoryTests
        {
            public class WhenCallNumberNotSet : WhenSettingBasicProperties
            {
                protected override void SetupMetadata()
                {
                    base.SetupMetadata();
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
            public void ItShouldSetBarcodeCorrectly()
            {
                Assert.That(Result.Barcode, Is.EqualTo(PodMetadata.Barcode));
            }

            [Test]
            public void ItShouldSetCarrierTypeCorrectly()
            {
                Assert.That(Result.CarrierType, Is.EqualTo("Video"));
            }
            
            [Test]
            public void ItShouldSetDefinitionCorrectly()
            {
                Assert.That(Result.Definition, Is.EqualTo("SD"));
            }

            [Test]
            public void ItShouldSetImageFormatCorrectly()
            {
                Assert.That(Result.ImageFormat, Is.EqualTo(PodMetadata.ImageFormat));
            }

            [Test]
            public void ItShouldSetRecordingStandardCorrectly()
            {
                Assert.That(Result.RecordingStandard, Is.EqualTo(PodMetadata.RecordingStandard));
            }

            [Test]
            public void ItShouldSetCommentsCorrectly()
            {
                Assert.That(Result.Comments, Is.EqualTo("comments value"));
            }
        }

        public class WhenSettingCleaningData : VideoCarrierDataFactoryTests
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

        public class WhenSettingBakingData : VideoCarrierDataFactoryTests
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
        
        public class WhenSettingPhysicalConditionData : VideoCarrierDataFactoryTests
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

        public class WhenSettingPartsData :VideoCarrierDataFactoryTests
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

        public class WhenSettingConfiguration : VideoCarrierDataFactoryTests
        {
            [Test]
            public void ConfigurationShouldNotBeNull()
            {
                Assert.That(Result.Configuration, Is.Not.Null);
            }

            [Test]
            public virtual void DigitialFileConfigurationVarientShouldBeSetCorrectly()
            {
                Assert.That(Result.Configuration.DigitalFileConfigurationVariant, Is.EqualTo(string.Empty));
            }

            [Test]
            public virtual void IsShouldCallMediaInfoProviderCorrectly()
            {
                MediaInfoProvider.DidNotReceive()
                       .GetMediaInfo(Arg.Any<AbstractFile>(), Arg.Any<CancellationToken>());
            }

            public class WhenFormatIsNotBetamax : WhenSettingConfiguration
            {
            }

            public class WhenFormatIsBetamax : WhenSettingConfiguration
            {
                protected override void SetupMetadata()
                {
                    base.SetupMetadata();
                    PodMetadata.Format = MediaFormats.Betamax;
                }

                [Test]
                public override void IsShouldCallMediaInfoProviderCorrectly()
                {
                    MediaInfoProvider.Received()
                        .GetMediaInfo(PreservationSide1FileModel, Arg.Any<CancellationToken>());
                }

                public class WhenPresObjectHasGreaterThanTwoAudioStreams : WhenFormatIsBetamax
                {
                    protected override void SetupMediaInfoProvider()
                    {
                        base.SetupMediaInfoProvider();
                        MediaInfoProvider.GetMediaInfo(PreservationSide1FileModel, Arg.Any<CancellationToken>())
                            .Returns(Task.FromResult(new MediaInfo {AudioStreams = 4}));
                    }
                    
                    [Test]
                    public override void DigitialFileConfigurationVarientShouldBeSetCorrectly()
                    {
                        Assert.That(Result.Configuration.DigitalFileConfigurationVariant, Is.EqualTo("4StreamAudio"));
                    }
                }

                public class WhenPresObjectHasLessThanFourAudioStreams : WhenFormatIsBetamax
                {
                    protected override void SetupMediaInfoProvider()
                    {
                        base.SetupMediaInfoProvider();
                        MediaInfoProvider.GetMediaInfo(PreservationSide1FileModel, Arg.Any<CancellationToken>())
                            .Returns(Task.FromResult(new MediaInfo {AudioStreams = 2}));
                    }
                }
            }
        }
    }
}