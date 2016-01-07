using System;

namespace Packager.Models.OutputModels
{
    [Serializable]
    public class VideoCarrierData : AbstractCarrierData
    {
        public string RecordingStandard { get; set; }
        public string ImageFormat { get; set; }
        public string Definition { get; set; }
    }
}