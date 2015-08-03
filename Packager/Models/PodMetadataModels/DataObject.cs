namespace Packager.Models.PodMetadataModels
{
    public class DataObject
    {
        public Basics Basics { get; set; }
        public Assignment Assignment { get; set; }
        public Details Details { get; set; }
        public TechnicalMetadata TechnicalMetadata { get; set; }
        public DigitalProvenance DigitalProvenance { get; set; }
    }
}