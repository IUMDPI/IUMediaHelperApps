using System;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace Packager.Models.OutputModels
{
    [Serializable]
    public class IngestData
    {
        [XmlAttribute("type", Form = XmlSchemaForm.Qualified, Namespace = "http://www.w3.org/2001/XMLSchema-instance")]
        public string XsiType { get; set; }
        
        [XmlElement(Order = 1)]
        public string Date { get; set; }

        [XmlElement(Order = 2)]
        public string Comments { get; set; }

        [XmlElement("Created_by", Order = 3)]
        public string CreatedBy { get; set; }

        [XmlElement("Player", Order = 4)]
        public IngestDevice[] Players { get; set; }

        [XmlElement("AD", Order = 5)]
        public IngestDevice[] AdDevices { get; set; }

        [XmlElement("Extraction_workstation", Order = 6)]
        public string ExtractionWorkstation { get; set; }

        [XmlElement("Speed_used", Order = 7)]
        public string SpeedUsed { get; set; }

        [XmlElement("Preamp", Order = 8)]
        public string PreAmp { get; set; }

        [XmlElement("Preamp_serial_number", Order = 9)]
        public string PreAmpSerialNumber { get; set; }
    }
}