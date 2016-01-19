using System;
using System.Xml.Linq;
using NSubstitute;
using NUnit.Framework;
using Packager.Factories;
using Packager.Models.PodMetadataModels;

namespace Packager.Test.Models.MetadataModels.PodMetadataTests
{
    [TestFixture]
    public abstract class AbstractPodMetadataTests
    {
        [SetUp]
        public virtual void BeforeEach()
        {
            Factory = Substitute.For<IImportableFactory>();
            Element = new XElement("pod");

            Factory.ResolveDamage(Element, "data/object/technical_metadata/damage").Returns("damage value");
            Factory.ResolvePreservationProblems(Element, "data/object/technical_metadata/preservation_problems")
                .Returns("preservation problems value");
            Factory.ToUtcDateTimeValue(Element, "data/object/digital_provenance/baking_date")
                .Returns(new DateTime(2016, 1, 1));
            Factory.ToStringValue(Element, "data/object/details/mdpi_barcode").Returns("barcode value");
            Factory.ToStringValue(Element, "data/object/details/call_number").Returns("call number value");

            Factory.ToStringValue(Element, "data/object/digital_provenance/cleaning_comment").Returns("cleaning comment value");
            Factory.ToUtcDateTimeValue(Element, "data/object/digital_provenance/cleaning_date")
                .Returns(new DateTime(2016, 2, 1));
            Factory.ToStringValue(Element, "data/object/digital_provenance/digitizing_entity").Returns("digitizing entity value");
            Factory.ToStringValue(Element, "data/object/details/id").Returns("identifier value");
            Factory.ToStringValue(Element, "data/object/digital_provenance/repaired").Returns("No"); //todo: fix this so it's a factory method and is more easily mockable

            Factory.ToStringValue(Element, "data/object/details/title").Returns("title value");
            Factory.ToStringValue(Element, "data/object/assignment/unit").Returns("unit value");
        }

        private string FormatValue { get; set; }
        private IImportableFactory Factory { get; set; }
        private XElement Element { get; set; }
        private AbstractPodMetadata Instance { get; set; }

        public class WhenImportingOpenReelAudioPodMetadata : AbstractPodMetadataTests
        {
         
            private AudioPodMetadata AudioPodMetadata => Instance as AudioPodMetadata;
            [SetUp]
            public override void BeforeEach()
            {
                base.BeforeEach();

                // need to make factory produce correct value for format
                FormatValue = "open reel audio tape";
                Factory.ToStringValue(Element, "data/object/details/format").Returns(FormatValue);
                
                Factory.ResolvePlaybackSpeed(Element, "data/object/technical_metadata/playback_speed").Returns("speed value");
                Factory.ResolveSoundField(Element, "data/object/technical_metadata/sound_field").Returns("sound field value");
                Factory.ResolveTapeThickness(Element, "data/object/technical_metadata/tape_thickness").Returns("tape thickness value");
                Factory.ResolveTrackConfiguration(Element, "data/object/technical_metadata/track_configuration").Returns("track configuration value");
                Factory.ToStringValue(Element, "data/object/technical_metadata/tape_stock_brand").Returns("brand value");
                Factory.ToStringValue(Element, "data/object/technical_metadata/directions_recorded").Returns("directions recorded value");

                Instance = new AudioPodMetadata();
                Instance.ImportFromXml(Element, Factory);
            }
            
            [Test]
            public void ItShouldCallFactoryToResolvePlaybackSpeed()
            {
                Factory.Received().ResolvePlaybackSpeed(Element, "data/object/technical_metadata/playback_speed");
            }

            [Test]
            public void ItShouldSetPlaybackSpeedCorrectly()
            {
                Assert.That(AudioPodMetadata.PlaybackSpeed, Is.EqualTo("speed value"));
            }

            [Test]
            public void ItShouldCallFactoryToResolveSoundField()
            {
                Factory.Received().ResolveSoundField(Element, "data/object/technical_metadata/sound_field");
            }

            [Test]
            public void ItShouldSetSoundFieldCorrectly()
            {
                Assert.That(AudioPodMetadata.SoundField, Is.EqualTo("sound field value"));
            }

            [Test]
            public void ItShouldCallFactoryToResolveTapeThickness()
            {
                Factory.Received().ResolveTapeThickness(Element, "data/object/technical_metadata/tape_thickness");
            }

            [Test]
            public void ItShouldSetTapeThicknessCorrectly()
            {
                Assert.That(AudioPodMetadata.TapeThickness, Is.EqualTo("tape thickness value"));
            }

            [Test]
            public void ItShouldCallFactoryToResolveTrackConfiguration()
            {
                Factory.Received()
                    .ResolveTrackConfiguration(Element, "data/object/technical_metadata/track_configuration");
            }

            [Test]
            public void ItShouldSetTrackConfigurationCorrectly()
            {
                Assert.That(AudioPodMetadata.TrackConfiguration, Is.EqualTo("track configuration value"));
            }

            [Test]
            public void ItShouldUseCorrectPathToResolveBrand()
            {
                Factory.Received().ToStringValue(Element, "data/object/technical_metadata/tape_stock_brand");
            }

            [Test]
            public void ItShouldSetBrandCorrectly()
            {
                Assert.That(AudioPodMetadata.Brand, Is.EqualTo("brand value"));
            }

            [Test]
            public void ItShouldUseCorrectPathToResolveDirectionsRecorded()
            {
                Factory.Received().ToStringValue(Element, "data/object/technical_metadata/directions_recorded");
            }

            [Test]
            public void ItShouldSetDirectionsRecordedCorrectly()
            {
                Assert.That(AudioPodMetadata.DirectionsRecorded, Is.EqualTo("directions recorded value"));
            }
        }

        public class WhenImportingLacquerDiscAudioPodMetadata : AbstractPodMetadataTests
        {
            private AudioPodMetadata AudioPodMetadata => Instance as AudioPodMetadata;

            [SetUp]
            public override void BeforeEach()
            {
                base.BeforeEach();

                // need to make factory produce correct value for format
                FormatValue = "lacquer disc";
                Factory.ToStringValue(Element, "data/object/details/format").Returns(FormatValue);
                
                Factory.ToStringValue(Element, "data/object/technical_metadata/speed", " rpm").Returns("speed value");
                Factory.ToStringValue(Element, "data/object/technical_metadata/sound_field").Returns("sound field value");

                Instance = new AudioPodMetadata();
                Instance.ImportFromXml(Element, Factory);
            }

            [Test]
            public void ItShouldCallFactoryToResolvePlaybackSpeed()
            {
                Factory.Received().ToStringValue(Element, "data/object/technical_metadata/speed", " rpm");
            }

            [Test]
            public void PlaybackSpeedShouldHaveCorrectValue()
            {
                Assert.That(AudioPodMetadata.PlaybackSpeed, Is.EqualTo("speed value"));
            }

            [Test]
            public void ItShouldCallFactoryToResolveSoundField()
            {
                Factory.Received().ToStringValue(Element, "data/object/technical_metadata/sound_field");
            }

            [Test]
            public void SoundfieldShouldHaveCorrectValue()
            {
                Assert.That(AudioPodMetadata.SoundField, Is.EqualTo("sound field value"));
            }
        }


        public class WhenImportingVideoPodMetadata : AbstractPodMetadataTests
        {
            public VideoPodMetadata VideoPodMetadata => Instance as VideoPodMetadata;

            [SetUp]
            public override void BeforeEach()
            {
                base.BeforeEach();

                // need to make factory produce correct value for format
                FormatValue = "8mm video";
                Factory.ToStringValue(Element, "data/object/details/format").Returns(FormatValue);
                Factory.ToStringValue(Element, "data/object/technical_metadata/image_format").Returns("image format value");
                Factory.ToStringValue(Element, "data/object/technical_metadata/format_version").Returns("definition value");
                Factory.ToStringValue(Element, "data/object/technical_metadata/recording_standard").Returns("recording standard value");

                Instance = new VideoPodMetadata();
                Instance.ImportFromXml(Element, Factory);
            }

            [Test]
            public void ItShouldCallFactoryCorrectlyToResolveImageFormat()
            {
                Factory.Received().ToStringValue(Element, "data/object/technical_metadata/image_format");
            }

            [Test]
            public void ItShouldSetImageFormatCorrectly()
            {
                Assert.That(VideoPodMetadata.ImageFormat, Is.EqualTo("image format value"));
            }

            [Test]
            public void ItShouldCallFactoryCorrectlyToResolveFormatVersion()
            {
                Factory.Received().ToStringValue(Element, "data/object/technical_metadata/format_version");
            }

            [Test]
            public void ItShouldSetDefinitionCorrectly()
            {
                Assert.That(VideoPodMetadata.Definition, Is.EqualTo("definition value"));
            }

            [Test]
            public void ItShouldCallFactoryCorrectlyToResolveRecordingStandard()
            {
                Factory.Received().ToStringValue(Element, "data/object/technical_metadata/recording_standard");
            }

            [Test]
            public void ItShouldSetFormatRecordingStandardCorrectly()
            {
                Assert.That(VideoPodMetadata.RecordingStandard, Is.EqualTo("recording standard value"));
            }
        }

        [Test]
        public void ItShouldCallFactoryCorrectlyToPreservationProblems()
        {
            Factory.Received()
                .ResolvePreservationProblems(Element, "data/object/technical_metadata/preservation_problems");
        }

        [Test]
        public void ItShouldSetPreservationProblemsCorrectly()
        {
            Assert.That(Instance.PreservationProblems, Is.EqualTo("preservation problems value"));
        }

        [Test]
        public void ItShouldCallFactoryCorrectlyToResolveDamage()
        {
            Factory.Received().ResolveDamage(Element, "data/object/technical_metadata/damage");
        }

        [Test]
        public void ItShouldSetDamageCorrectly()
        {
            Assert.That(Instance.Damage, Is.EqualTo("damage value"));
        }

        [Test]
        public void ItShouldUseCorrectPathToResolveBakingDate()
        {
            Factory.Received().ToUtcDateTimeValue(Element, "data/object/digital_provenance/baking_date");
        }

        [Test]
        public void ItShouldSetBakingDateCorrectly()
        {
            Assert.That(Instance.BakingDate, Is.EqualTo(new DateTime(2016,1,1)));
        }

        [Test]
        public void ItShouldUseCorrectPathToResolveBarcode()
        {
            Factory.Received().ToStringValue(Element, "data/object/details/mdpi_barcode");
        }

        [Test]
        public void ItShouldSetBarcodeCorrectly()
        {
            Assert.That(Instance.Barcode, Is.EqualTo("barcode value"));
        }

        [Test]
        public void ItShouldUseCorrectPathToResolveCallNumber()
        {
            Factory.Received().ToStringValue(Element, "data/object/details/call_number");
        }

        [Test]
        public void ItShouldSetCallNumberCorrectly()
        {
            Assert.That(Instance.CallNumber, Is.EqualTo("call number value"));
        }

        [Test]
        public void ItShouldUseCorrectPathToResolveCleaningComment()
        {
            Factory.Received().ToStringValue(Element, "data/object/digital_provenance/cleaning_comment");
        }

        [Test]
        public void ItShouldSetCleaningCommentCorrectly()
        {
            Assert.That(Instance.CleaningComment, Is.EqualTo("cleaning comment value"));
        }

        [Test]
        public void ItShouldUseCorrectPathToResolveCleaningDate()
        {
            Factory.Received().ToUtcDateTimeValue(Element, "data/object/digital_provenance/cleaning_date");
        }

        [Test]
        public void ItShouldSetCleaningDateCorrectly()
        {
            Assert.That(Instance.CleaningDate, Is.EqualTo(new DateTime(2016,2,1)));
        }

        [Test]
        public void ItShouldUseCorrectPathToResolveDigitizingEntity()
        {
            Factory.Received().ToStringValue(Element, "data/object/digital_provenance/digitizing_entity");
        }

        [Test]
        public void ItShouldSetDigitizingEntityCorrectly()
        {
            Assert.That(Instance.DigitizingEntity, Is.EqualTo("digitizing entity value"));
        }

        [Test]
        public void ItShouldUseCorrectPathToResolveFormet()
        {
            Factory.Received().ToStringValue(Element, "data/object/details/format");
        }

        [Test]
        public void ItShouldSetFormatCorrectly()
        {
            Assert.That(Instance.Format, Is.EqualTo(FormatValue));
        }

        [Test]
        public void ItShouldUseCorrectPathToResolveIdentifier()
        {
            Factory.Received().ToStringValue(Element, "data/object/details/id");
        }

        [Test]
        public void ItShouldSetIdentifierCorrectly()
        {
            Assert.That(Instance.Identifier, Is.EqualTo("identifier value"));
        }

        [Test]
        public void ItShouldUseCorrectPathToResolveRepaired()
        {
            Factory.Received().ToBooleanValue(Element, "data/object/digital_provenance/repaired");
        }

        [Test]
        public void ItShouldSetRepairedCorrectly()
        {
            Assert.That(Instance.Repaired, Is.EqualTo("No"));
        }

        [Test]
        public void ItShouldUseCorrectPathToResolveTitle()
        {
            Factory.Received().ToStringValue(Element, "data/object/details/title");
        }

        [Test]
        public void ItShouldSetTitleCorrectly()
        {
            Assert.That(Instance.Title, Is.EqualTo("title value"));
        }

        [Test]
        public void ItShouldUseCorrectPathToResolveUnit()
        {
            Factory.Received().ToStringValue(Element, "data/object/assignment/unit");
        }

        [Test]
        public void ItShouldSetUnitCorrectly()
        {
            Assert.That(Instance.Unit, Is.EqualTo("unit value"));
        }
    }
}