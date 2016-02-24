using System;
using System.Xml.Serialization;

namespace Packager.Models.OutputModels.Ingest
{
    [Serializable]
    public class VideoIngest : AbstractIngest
    {
        [XmlElement(Order = 1)]
        public string FileName { get; set; }

        [XmlElement(Order = 2)]
        public string Date { get; set; }

        [XmlElement(Order = 3)]
        public string DigitStatus { get; set; }

        [XmlElement(Order = 4)]
        public string Comments { get; set; }

        [XmlElement(ElementName = "Created_by", Order = 5)]
        public string CreatedBy { get; set; }

        [XmlElement(Order = 6, ElementName = "Player")]
        public Device[] Players { get; set; }

        [XmlElement(Order = 7, ElementName = "TBC")]
        public Device[] TbcDevices { get; set; }

        [XmlElement(Order = 8, ElementName = "AD")]
        public Device[] AdDevices { get; set; }

        [XmlElement(Order = 9)]
        public Device Encoder { get; set; }

        [XmlElement(Order = 10, ElementName = "Extraction_workstation")]
        public Device ExtractionWorkstation { get; set; }

        
    }
}