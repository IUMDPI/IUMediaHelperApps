using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Packager.Extensions;
using Packager.Validators.Attributes;

namespace Packager.Models.PodMetadataModels
{
    public class AudioPodMetadata : AbstractPodMetadata
    {
        private static readonly Dictionary<string, string> KnownPlaybackSpeeds = new Dictionary<string, string>
        {
            {"zero_point9375_ips", ".9375 ips"},
            {"one_point875_ips", "1.875 ips"},
            {"three_point75_ips", "3.75 ips"},
            {"seven_point5_ips", "7.5 ips"},
            {"fifteen_ips", "15 ips"},
            {"thirty_ips", "30 ips"},
            {"unknown_playback_speed", "Unknown"}
        };

        private static readonly Dictionary<string, string> KnownTrackConfigurations = new Dictionary<string, string>
        {
            {"full_track", "Full track"},
            {"half_track", "Half track"},
            {"quarter_track", "Quarter track"},
            {"unknown_track", "Unknown"}
        };

        private static readonly Dictionary<string, string> KnownSoundFields = new Dictionary<string, string>
        {
            {"mono", "Mono"},
            {"stereo", "Stereo"},
            {"unknown_sound_field", "unknown"}
        };

        private static readonly Dictionary<string, string> KnownTapeThicknesses = new Dictionary<string, string>
        {
            {"zero_point5_mils", ".5"},
            {"one_mils", "1.0"},
            {"one_point5_mils", "1.5"}
        };

        public string Brand { get; set; }

        public string DirectionsRecorded { get; set; }

        public string PlaybackSpeed { get; set; }

        public string TrackConfiguration { get; set; }

        [Required]
        public string SoundField { get; set; }

        public string TapeThickness { get; set; }

        public override void ImportFromXml(XElement element)
        {
            base.ImportFromXml(element);
            Brand = element.ToStringValue("data/object/technical_metadata/tape_stock_brand");
            DirectionsRecorded = element.ToStringValue("data/object/technical_metadata/directions_recorded");
            PlaybackSpeed = element.ToResolvedDelimitedString("data/object/technical_metadata/playback_speed", KnownPlaybackSpeeds);
            TrackConfiguration = element.ToResolvedDelimitedString("data/object/technical_metadata/track_configuration", KnownTrackConfigurations);
            SoundField = element.ToResolvedDelimitedString("data/object/technical_metadata/sound_field", KnownSoundFields);
            TapeThickness = element.ToResolvedDelimitedString("data/object/technical_metadata/tape_thickness", KnownTapeThicknesses);
        }

        protected override List<AbstractDigitalFile> ImportFileProvenances(IEnumerable<XElement> elements)
        {
            return elements.Select(element => element.ToImportable<DigitalAudioFile>())
                .Cast<AbstractDigitalFile>().ToList();
        }
    }
}