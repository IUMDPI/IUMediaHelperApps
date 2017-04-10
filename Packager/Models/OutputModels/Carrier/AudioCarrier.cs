using System;
using System.Xml.Serialization;

namespace Packager.Models.OutputModels.Carrier
{
    [Serializable]
    public class AudioCarrier:AbstractCarrierData
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
        public AudioConfigurationData Configuration { get; set; }

        [XmlElement(Order = 5)]
        public string Brand { get; set; }

        [XmlElement(Order = 6)]
        public string Thickness { get; set; }

        [XmlElement(Order = 7)]
        public string DirectionsRecorded { get; set; }

        [XmlElement(Order = 8)]
        public string Repaired { get; set; }

        [XmlElement(Order = 9)]
        public PartsData Parts { get; set; }

        [XmlElement(Order = 10)]
        public PreviewData Preview { get; set; }

        [XmlElement(Order = 11)]
        public string Comments { get; set; }

        [XmlElement(Order = 12)]
        public CleaningData Cleaning { get; set; }

        [XmlElement(Order = 13)]
        public BakingData Baking { get; set; }
    }
}