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

        [XmlElement(Order = 200)]
        public PartsData Parts { get; set; }

        [XmlElement(Order = 400)]
        public PreviewData Preview { get; set; }

        [XmlElement(Order = 401)]
        public CleaningData Cleaning { get; set; }

        [XmlElement(Order = 402)]
        public BakingData Baking { get; set; }

        [XmlElement(Order=100)]
        public ConfigurationData Configuration { get; set; }

        [XmlElement(Order = 101)]
        public string Brand { get; set; }

        [XmlElement(Order = 102)]
        public string Thickness { get; set; }

        [XmlElement(Order = 103)]
        public string DirectionsRecorded { get; set; }

        [XmlElement(Order = 104)]
        public string Repaired { get; set; }
    }
}