using System;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace Packager.Models.OutputModels
{
    [Serializable]
    public abstract class AbstractCarrierData
    {
        [XmlAttribute("type")]
        public string CarrierType { get; set; }

        [XmlAttribute("type", Form = XmlSchemaForm.Qualified, Namespace = "http://www.w3.org/2001/XMLSchema-instance")]
        public string XsiType { get; set; }

        [XmlElement(Order = 1)]
        public string Identifier { get; set; }

        [XmlElement(Order = 2)]
        public string Barcode { get; set; }

        [XmlElement(Order = 12)]
        public PartsData Parts { get; set; }

        [XmlElement(Order = 7)]
        public PhysicalConditionData PhysicalCondition { get; set; }

        [XmlElement(Order = 9)]
        public PreviewData Preview { get; set; }

        [XmlElement(Order = 10)]
        public CleaningData Cleaning { get; set; }

        [XmlElement(Order = 11)]
        public BakingData Baking { get; set; }
    }
}