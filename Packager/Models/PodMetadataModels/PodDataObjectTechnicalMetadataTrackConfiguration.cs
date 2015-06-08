namespace Packager.Models.PodMetadataModels
{
    public class PodDataObjectTechnicalMetadataTrackConfiguration
    {
        public bool FullTrack { get; set; }
        public bool HalfTrack { get; set; }


        public string GetValue()
        {
            if (FullTrack)
            {
                return "Full track";
            }

            if (HalfTrack)
            {
                return "Half track";
            }

            return "Unknown";
        }
    }
}