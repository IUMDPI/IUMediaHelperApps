using System;
using System.Xml.Serialization;

namespace Packager.Models.OutputModels
{
    [Serializable]
    public class CarrierData
    {
        [XmlAttribute("type")]
        public string CarrierType { get; set; }

        [XmlAttribute("xsi:type")]
        public string XsiType { get; set; }

        // todo: figure out where to get this from
        [XmlElement(Order = 1)]
        public string Identifier { get; set; }

        [XmlElement(Order = 2)]
        public string Barcode { get; set; }

        [XmlElement(Order = 3)]
        public ConfigurationData Configuration { get; set; }

        [XmlElement(Order = 4)]
        public string Brand { get; set; }

        [XmlElement(Order = 5)]
        public string Thickness { get; set; }

        [XmlElement(Order = 6)]
        public string DirectionsRecorded { get; set; }

        [XmlElement(Order = 7)]
        public PhysicalConditionData PhysicalCondition { get; set; }

        [XmlElement(Order = 8)]
        public string Repaired { get; set; }

        [XmlElement(Order = 9)]
        public PreviewData Preview { get; set; }

        [XmlElement(Order = 10)]
        public CleaningData Cleaning { get; set; }

        [XmlElement(Order = 11)]
        public BakingData Baking { get; set; }

        [XmlElement(Order = 12)]
        public PartsData Parts { get; set; }
    }
}