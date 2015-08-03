using System.ComponentModel;

namespace Packager.Models.PodMetadataModels
{
    public class TrackConfiguration
    {
        [Description("Full track")]
        public bool FullTrack { get; set; }

        [Description("Full track")]
        public bool HalfTrack { get; set; }

        [Description("Full track")]
        public bool QuarterTrack { get; set; }

        [Description("Unknown")]
        public bool UnknownTrack { get; set; }
    }
}