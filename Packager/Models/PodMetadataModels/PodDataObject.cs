namespace Packager.Models.PodMetadataModels
{
    public class PodDataObject
    {
        public PodDataObjectBasics Basics { get; set; }
        public PodDataObjectAssignment Assignment { get; set; }
        public PodDataObjectDetails Details { get; set; }
        public PodDataObjectTechnicalMetadata TechnicalMetadata { get; set; }
        public object DigitalProvenance { get; set; }
    }
}