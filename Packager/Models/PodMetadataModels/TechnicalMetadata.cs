namespace Packager.Models.PodMetadataModels
{
    public class TechnicalMetadata
    {
        public string Format { get; set; }
        public int Files { get; set; }
        public Damage Damage { get; set; }
        public PlaybackSpeed PlaybackSpeed { get; set; }
        public PreservationProblems PreservationProblems { get; set; }
        public SampleRate SampleRate { get; set; }
        public SoundField SoundField { get; set; }
        public TapeBase TapeBase { get; set; }
        public TrackConfiguration TrackConfiguration { get; set; }
        public TapeThickness TapeThickness { get; set; }
        public string CassetteSize { get; set; }
        public string Coating { get; set; }
        public string CountryOfOrigin { get; set; }
        public string Diameter { get; set; }
        public string DirectionsRecorded { get; set; }
        public string Equalization { get; set; }
        public string FormatDuration { get; set; }
        public string FormatVersion { get; set; }
        public string GrooveOrientation { get; set; }
        public string GrooveSize { get; set; }
        public string ImageFormat { get; set; }
        public string Label { get; set; }
        public string Material { get; set; }
        public string PackDeformation { get; set; }
        public string RecordingMethod { get; set; }
        public string RecordingStandard { get; set; }
        public string ReelSize { get; set; }
        public string Speed { get; set; }
        public string Substrate { get; set; }
        public string TapeStockBrand { get; set; }
    }
}