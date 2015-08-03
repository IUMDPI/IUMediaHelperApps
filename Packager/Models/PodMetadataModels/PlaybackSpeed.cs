using System.ComponentModel;

namespace Packager.Models.PodMetadataModels
{
    public class PlaybackSpeed
    {
        [Description(".9375 ips")]
        public bool ZeroPoint9375Ips { get; set; }

        [Description("1.875 ips")]
        public bool OnePoint875Ips { get; set; }

        [Description("3.75 ips")]
        public bool ThreePoint75Ips { get; set; }
        
        [Description("7.5 ips")]
        public bool SevenPoint5Ips { get; set; }

        [Description("15 ips")]
        public bool FifteenIps { get; set; }

        [Description("30 ips")]
        public bool ThirtyIps { get; set; }

        [Description("Unknown")]
        public bool UnknownPlaybackSpeed { get; set; }
    }
}