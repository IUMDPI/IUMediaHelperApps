using System;
using System.Xml.Serialization;
using Packager.Attributes;

namespace Packager.Models
{
    [Serializable]
    public class CarrierData
    {
        [ExcelField("Carrier Type", true)]
        [XmlAttribute("type")]
        public string CarrierType { get; set; }

        [ExcelField("Identifier", true)]
        [XmlElement(Order = 1)]
        public string Identifier { get; set; }

        [ExcelField("Barcode", true)]
        [XmlElement(Order = 2)]
        public string Barcode { get; set; }

        [XmlElement(Order = 4)]
        [ExcelField("Brand", true)]
        public string Brand { get; set; }

        [ExcelField("Thickness", true)]
        [XmlElement(Order = 5)]
        public string Thickness { get; set; }

        [ExcelField("DirectionsRecorded", true)]
        [XmlElement(Order = 6)]
        public string DirectionsRecorded { get; set; }

        [XmlElement(Order = 8)]
        [ExcelField("Repaired", true)]
        public string Repaired { get; set; }

        [ExcelObject]
        [XmlElement(Order = 3)]
        public ConfigurationData Configuration { get; set; }

        [ExcelObject]
        [XmlElement(Order = 7)]
        public PhysicalConditionData PhysicalCondition { get; set; }

        [ExcelObject]
        [XmlElement(Order = 9)]
        public PreviewData Preview { get; set; }

        [ExcelObject]
        [XmlElement(Order = 10)]
        public CleaningData Cleaning { get; set; }

        [ExcelObject]
        [XmlElement(Order = 11)]
        public BakingData Baking { get; set; }

        [ExcelObject]
        [XmlElement(Order = 12)]
        public PartsData Parts { get; set; }
    }
}