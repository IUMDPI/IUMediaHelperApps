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

        public static ConfigurationData FromPodMetadata(PodMetadata podMetadata)
        {
            return new ConfigurationData
            {
                Track = podMetadata.Data.Object.TechnicalMetadata.TrackConfiguration.GetValue(),
                SoundField = podMetadata.Data.Object.TechnicalMetadata.SoundField.GetValue(),
                Speed = podMetadata.Data.Object.TechnicalMetadata.PlaybackSpeed.GetValue()
            };
        }


    }
}