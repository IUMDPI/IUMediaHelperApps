using System;
using System.Xml.Serialization;

namespace Packager.Models.OutputModels.Ingest
{
    [Serializable]
    public class VideoIngest : AbstractIngest
    {
        [XmlElement(Order = 1)]
        public string Date { get; set; }

        [XmlElement(Order = 2)]
        public string DigitStatus { get; set; }

        [XmlElement(Order = 3)]
        public string Comments { get; set; }

        [XmlElement(ElementName = "Created_by", Order = 4)]
        public string CreatedBy { get; set; }

        [XmlElement(Order = 5)]
        public Device[] Players { get; set; }

        [XmlElement(Order = 6)]
        public Device[] TbcDevices { get; set; }

        [XmlElement(Order = 7)]
        public Device[] AdDevices { get; set; }

        [XmlElement(Order = 8)]
        public Device ExtractionWorkstation { get; set; }

        [XmlElement(Order = 9)]
        public Device Encoder { get; set; }
    }
}