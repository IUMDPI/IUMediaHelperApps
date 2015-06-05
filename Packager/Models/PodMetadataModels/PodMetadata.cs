namespace Packager.Models.PodMetadataModels
{
    public class PodMetadata
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public PodData Data { get; set; }
    }
}