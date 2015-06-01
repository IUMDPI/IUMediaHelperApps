using System;
using System.Xml.Serialization;

namespace Packager.Models
{
    [Serializable]
    public class IngestData
    {
        [XmlElement(Order = 1)]
        public string Date { get; set; }

        [XmlElement(Order = 2)]
        public string Comments { get; set; }

        [XmlElement("Created_by", Order = 3)]
        public string CreatedBy { get; set; }

        [XmlElement("Player_serial_number", Order = 4)]
        public string PlayerSerialNumber { get; set; }

        [XmlElement("Player_manufacturer", Order = 5)]
        public string PlayerManufacturer { get; set; }

        [XmlElement("Player_model", Order = 6)]
        public string PlayerModel { get; set; }

        [XmlElement("AD_serial_number", Order = 7)]
        public string AdSerialNumber { get; set; }

        [XmlElement("AD_manufacturer", Order = 8)]
        public string AdManufacturer { get; set; }

        [XmlElement("AD_model", Order = 9)]
        public string AdModel { get; set; }

        [XmlElement("Extraction_workstation", Order = 10)]
        public string ExtractionWorkstation { get; set; }

        [XmlElement("Speed_used", Order = 11)]
        public string SpeedUsed { get; set; }
    }
}