namespace Packager.Models.PodMetadataModels
{
    public class PodDataObjectTechnicalMetadataSoundField
    {
        public bool Mono { get; set; }
        public bool Stereo { get; set; }

        public string GetValue()
        {
            if (Mono)
            {
                return "Mono";
            }

            if (Stereo)
            {
                return "Stereo";
            }

            return "Unknown";
        }
    }
}