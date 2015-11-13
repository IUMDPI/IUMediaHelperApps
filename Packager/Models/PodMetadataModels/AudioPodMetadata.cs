using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Packager.Extensions;
using Packager.Providers;
using Packager.Validators.Attributes;

namespace Packager.Models.PodMetadataModels
{
    public class AudioPodMetadata : AbstractPodMetadata
    {
        public string Brand { get; set; }

        public string DirectionsRecorded { get; set; }

        public string PlaybackSpeed { get; set; }

        public string TrackConfiguration { get; set; }

        [Required]
        public string SoundField { get; set; }

        public string TapeThickness { get; set; }

        public override void ImportFromXml(XElement element, ILookupsProvider lookupsProvider)
        {
            base.ImportFromXml(element, lookupsProvider);
            Brand = element.ToStringValue("data/object/technical_metadata/tape_stock_brand");
            DirectionsRecorded = element.ToStringValue("data/object/technical_metadata/directions_recorded");
            PlaybackSpeed = element.ToResolvedDelimitedString("data/object/technical_metadata/playback_speed", lookupsProvider.PlaybackSpeed);
            TrackConfiguration = element.ToResolvedDelimitedString("data/object/technical_metadata/track_configuration", lookupsProvider.TrackConfiguration);
            SoundField = element.ToResolvedDelimitedString("data/object/technical_metadata/sound_field", lookupsProvider.SoundField);
            TapeThickness = element.ToResolvedDelimitedString("data/object/technical_metadata/tape_thickness", lookupsProvider.TapeThickness);
        }

        protected override List<AbstractDigitalFile> ImportFileProvenances(IEnumerable<XElement> elements, ILookupsProvider lookupsProvider)
        {
            return elements.Select(element => element.ToImportable<DigitalAudioFile>(lookupsProvider))
                .Cast<AbstractDigitalFile>().ToList();
        }
    }
}