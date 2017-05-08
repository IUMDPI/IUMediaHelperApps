using System;
using System.Xml.Serialization;

namespace Packager.Models.OutputModels.Carrier
{
    [Serializable]
    public class VideoCarrier : AbstractCarrierData
    {
        [XmlAttribute("type")]
        public string CarrierType { get; set; }

        [XmlElement(Order = 1)]
        public string Identifier { get; set; }

        [XmlElement(Order = 2)]
        public string Barcode { get; set; }

        [XmlElement(Order = 3)]
        public PhysicalConditionData PhysicalCondition { get; set; }

        [XmlElement(Order = 4)]
        public VideoConfigurationData Configuration { get; set; }

        [XmlElement(Order = 5)]
        public PartsData Parts { get; set; }

        [XmlElement(Order = 6)]
        public string RecordingStandard { get; set; }

        [XmlElement(Order = 7)]
        public string ImageFormat { get; set; }

        [XmlElement(Order = 8)]
        public string Definition { get; set; }

        [XmlElement(Order = 9)]
        public PreviewData Preview { get; set; }

        [XmlElement(Order=10)]
        public string Comments { get; set; }

        [XmlElement(Order = 11)]
        public CleaningData Cleaning { get; set; }

        [XmlElement(Order = 12)]
        public BakingData Baking { get; set; }

       
    }
}