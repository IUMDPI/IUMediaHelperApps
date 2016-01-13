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
        }

        public IImportableFactory Factory { get; set; }
        public XElement Element { get; set; }


        public class WhenImportingOpenReelAudioPodMetadata : AbstractPodMetadataTests
        {
            [SetUp]
            public override void BeforeEach()
            {
                base.BeforeEach();

                // need to make factory produce correct value for format
                Factory.ToStringValue(Element, "data/object/details/format").Returns("open reel audio tape");

                var instance = new AudioPodMetadata();
                instance.ImportFromXml(Element, Factory);
            }


            [Test]
            public void ItShouldCallFactoryToResolvePlaybackSpeed()
            {
                Factory.Received().ResolvePlaybackSpeed(Element, "data/object/technical_metadata/playback_speed");
            }

            [Test]
            public void ItShouldCallFactoryToResolveSoundField()
            {
                Factory.Received().ResolveSoundField(Element, "data/object/technical_metadata/sound_field");
            }

            [Test]
            public void ItShouldCallFactoryToResolveTapeThickness()
            {
                Factory.Received().ResolveTapeThickness(Element, "data/object/technical_metadata/tape_thickness");
            }

            [Test]
            public void ItShouldCallFactoryToResolveTrackConfiguration()
            {
                Factory.Received()
                    .ResolveTrackConfiguration(Element, "data/object/technical_metadata/track_configuration");
            }

            [Test]
            public void ItShouldUseCorrectPathToResolveBrand()
            {
                Factory.Received().ToStringValue(Element, "data/object/technical_metadata/tape_stock_brand");
            }

            [Test]
            public void ItShouldUseCorrectPathToResolveDirectionsRecorded()
            {
                Factory.Received().ToStringValue(Element, "data/object/technical_metadata/directions_recorded");
            }
        }

        public class WhenImportingLacquerDiscAudioPodMetadata : AbstractPodMetadataTests
        {
            [SetUp]
            public override void BeforeEach()
            {
                base.BeforeEach();

                // need to make factory produce correct value for format
                Factory.ToStringValue(Element, "data/object/details/format").Returns("lacquer disc");

                var instance = new AudioPodMetadata();
                instance.ImportFromXml(Element, Factory);
            }

            [Test]
            public void ItShouldCallFactoryToResolvePlaybackSpeed()
            {
                Factory.Received().ToStringValue(Element, "data/object/technical_metadata/speed", " rpm");
            }

            [Test]
            public void ItShouldCallFactoryToResolveSoundField()
            {
                Factory.Received().ToStringValue(Element, "data/object/technical_metadata/sound_field");
            }
        }


        public class WhenImportingVideoPodMetadata : AbstractPodMetadataTests
        {
            [SetUp]
            public override void BeforeEach()
            {
                base.BeforeEach();

                var instance = new VideoPodMetadata();
                instance.ImportFromXml(Element, Factory);
            }

            [Test]
            public void ItShouldCallFactoryCorrectlyToResolveImageFormat()
            {
                Factory.Received().ToStringValue(Element, "data/object/technical_metadata/image_format");
            }


            [Test]
            public void ItShouldCallFactoryCorrectlyToResolveRecordingFormatVersion()
            {
                Factory.Received().ToStringValue(Element, "data/object/technical_metadata/format_version");
            }

            [Test]
            public void ItShouldCallFactoryCorrectlyToResolveRecordingStandard()
            {
                Factory.Received().ToStringValue(Element, "data/object/technical_metadata/recording_standard");
            }
        }

        [Test]
        public void ItShouldCallFactoryCorrectlyToPreservationProblems()
        {
            Factory.Received()
                .ResolvePreservationProblems(Element, "data/object/technical_metadata/preservation_problems");
        }

        [Test]
        public void ItShouldCallFactoryCorrectlyToResolveDamage()
        {
            Factory.Received().ResolveDamage(Element, "data/object/technical_metadata/damage");
        }

        [Test]
        public void ItShouldUseCorrectPathToResolveBakingDate()
        {
            Factory.Received().ToDateTimeValue(Element, "data/object/digital_provenance/baking_date");
        }

        [Test]
        public void ItShouldUseCorrectPathToResolveBarcode()
        {
            Factory.Received().ToStringValue(Element, "data/object/details/mdpi_barcode");
        }

        [Test]
        public void ItShouldUseCorrectPathToResolveCallNumber()
        {
            Factory.Received().ToStringValue(Element, "data/object/details/call_number");
        }

        [Test]
        public void ItShouldUseCorrectPathToResolveCleaningComment()
        {
            Factory.Received().ToStringValue(Element, "data/object/digital_provenance/cleaning_comment");
        }

        [Test]
        public void ItShouldUseCorrectPathToResolveCleaningDate()
        {
            Factory.Received().ToDateTimeValue(Element, "data/object/digital_provenance/cleaning_date");
        }

        [Test]
        public void ItShouldUseCorrectPathToResolveDigitizingEntity()
        {
            Factory.Received().ToStringValue(Element, "data/object/digital_provenance/digitizing_entity");
        }

        [Test]
        public void ItShouldUseCorrectPathToResolveFormet()
        {
            Factory.Received().ToStringValue(Element, "data/object/details/format");
        }

        [Test]
        public void ItShouldUseCorrectPathToResolveIdentifier()
        {
            Factory.Received().ToStringValue(Element, "data/object/details/id");
        }

        [Test]
        public void ItShouldUseCorrectPathToResolveRepaired()
        {
            Factory.Received().ToBooleanValue(Element, "data/object/digital_provenance/repaired");
        }

        [Test]
        public void ItShouldUseCorrectPathToResolveTitle()
        {
            Factory.Received().ToStringValue(Element, "data/object/details/title");
        }

        [Test]
        public void ItShouldUseCorrectPathToResolveUnit()
        {
            Factory.Received().ToStringValue(Element, "data/object/assignment/unit");
        }
    }
}