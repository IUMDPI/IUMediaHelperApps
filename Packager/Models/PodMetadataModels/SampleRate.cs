using System.ComponentModel;

namespace Packager.Models.PodMetadataModels
{
    public class SampleRate
    {
        [Description("32k")]
        public bool SampleRate32K { get; set; }

        [Description("44.1k")]
        public bool SampleRate441K { get; set; }

        [Description("48k")]
        public bool SampleRate48K { get; set; }

        [Description("96k")]
        public bool SampleRate96K { get; set; }
    }
}