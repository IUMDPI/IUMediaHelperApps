namespace Packager.Models.PodMetadataModels
{
    public class PodDataObjectTechnicalMetadata
    {
        public string PackDeformation { get; set; }
        public string ReelSize { get; set; }
        public string TapeStockBrand { get; set; }
        public byte DirectionsRecorded { get; set; }
        public object PreservationProblems { get; set; }
        public PodDataObjectTechnicalMetadataPlaybackSpeed PlaybackSpeed { get; set; }
        public PodDataObjectTechnicalMetadataTrackConfiguration TrackConfiguration { get; set; }
        public PodDataObjectTechnicalMetadataTapeThickness TapeThickness { get; set; }
        public PodDataObjectTechnicalMetadataSoundField SoundField { get; set; }
        public PodDataObjectTechnicalMetadataTapeBase TapeBase { get; set; }
    }
}