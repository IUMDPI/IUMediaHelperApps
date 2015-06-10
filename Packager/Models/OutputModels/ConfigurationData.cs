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
                Track = GetTrackConfiguration(podMetadata),
                SoundField = GetSoundField(podMetadata),
                Speed = GetSpeed(podMetadata)
            };
        }

        private static string GetTrackConfiguration(PodMetadata podMetadata)
        {
            return podMetadata.Data.Object.TechnicalMetadata.TrackConfiguration == null 
                ? null 
                : podMetadata.Data.Object.TechnicalMetadata.TrackConfiguration.GetValue();
        }

        private static string GetSoundField(PodMetadata podMetadata)
        {
            return podMetadata.Data.Object.TechnicalMetadata.SoundField == null
                ? null
                : podMetadata.Data.Object.TechnicalMetadata.SoundField.GetValue();
        }

        private static string GetSpeed(PodMetadata podMetadata)
        {
            return podMetadata.Data.Object.TechnicalMetadata.PlaybackSpeed == null
                ? null
                : podMetadata.Data.Object.TechnicalMetadata.PlaybackSpeed.GetValue();
        }
    }
}