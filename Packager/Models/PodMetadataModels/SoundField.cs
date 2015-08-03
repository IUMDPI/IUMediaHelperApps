using System.ComponentModel;

namespace Packager.Models.PodMetadataModels
{
    public class SoundField
    {
        [Description("Mono")]
        public bool Mono { get; set; }

        [Description("Stereo")]
        public bool Stereo { get; set; }

        [Description("Unknown")]
        public bool UnknownSoundField { get; set; }
    }
}