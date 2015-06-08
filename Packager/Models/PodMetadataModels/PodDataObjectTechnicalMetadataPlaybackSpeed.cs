namespace Packager.Models.PodMetadataModels
{
    public class PodDataObjectTechnicalMetadataPlaybackSpeed
    {
        public bool SevenPoint5Ips { get; set; }

        public string GetValue()
        {
            return SevenPoint5Ips ? "7.5 ips" : "Unknown";
        }
    }
}