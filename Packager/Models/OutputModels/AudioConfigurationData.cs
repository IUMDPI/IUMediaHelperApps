using System;

namespace Packager.Models.OutputModels
{
    [Serializable]
    public class AudioConfigurationData
    {
        public string Track { get; set; }
        public string SoundField { get; set; }
        public string Speed { get; set; }
        public string RecordingType { get; set; }
    }
}