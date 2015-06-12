using System;
using Packager.Models.PodMetadataModels;

namespace Packager.Models.OutputModels
{
    [Serializable]
    public class ConfigurationData
    {
        public string Track { get; set; }
        public string SoundField { get; set; }
        public string Speed { get; set; }

        public static ConfigurationData FromPodMetadata(ConsolidatedPodMetadata podMetadata)
        {
            return new ConfigurationData
            {
                Track = string.Join(",", podMetadata.TrackConfigurations),
                SoundField = string.Join(",", podMetadata.SoundFields),
                Speed = string.Join(",", podMetadata.PlaybackSpeeds)
            };
        }
    }
}