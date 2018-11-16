using System;
using RestSharp.Extensions;

namespace Packager.Models.OutputModels
{
    [Serializable]
    public class AudioConfigurationData
    {
        public string Track { get; set; }
        public string SoundField { get; set; }
        public string Speed { get; set; }
        public string RecordingType { get; set; }
        
        public string TapeType { get; set; }
        public string TapeStockBrand { get; set; }
        public string NoiseReduction { get; set; }
        public string FormatDuration { get; set; }

        public bool ShouldSerializeTrack() => Track.HasValue();
        public bool ShouldSerializeSoundField() => SoundField.HasValue();
        public bool ShouldSerializeSpeed() => Speed.HasValue();
        public bool ShouldSerializeRecordingType() => RecordingType.HasValue();
        public bool ShouldSerializeTapeType() => TapeType.HasValue();
        public bool ShouldSerializeTapeStockBrand() => TapeStockBrand.HasValue();
        public bool ShouldSerializeNoiseReduction() => NoiseReduction.HasValue();
        public bool ShouldSerializeFormatDuration() => FormatDuration.HasValue();
    }
}