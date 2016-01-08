using System;
using System.Xml.Serialization;

namespace Packager.Models.OutputModels.Ingest
{
    [Serializable]
    public class AudioIngest:AbstractIngest
    {
        public string Date { get; set; }

        public string Comments { get; set; }

        [XmlElement(ElementName = "Created_by")]
        public string CreatedBy { get; set; }

        public Device[] Players { get; set; }

        public Device[] AdDevices { get; set; }

        public Device ExtractionWorkstation { get; set; }

        [XmlElement("Speed_used")]
        public string SpeedUsed { get; set; }

        [XmlElement("Preamp")]
        public string PreAmp { get; set; }

        [XmlElement("Preamp_serial_number")]
        public string PreAmpSerialNumber { get; set; }

        [XmlElement("Stylus")]
        public string Stylus { get; set; }

        [XmlElement("Turnover")]
        public string Turnover { get; set; }

        [XmlElement("ReferenceFluxivity")]
        public string ReferenceFluxivity { get; set; }

        [XmlElement("Gain")]
        public string Gain { get; set; }

        [XmlElement("AnalogOutputVoltage")]
        public string AnalogOutputVoltage { get; set; }

        [XmlElement("Peak")]
        public string Peak { get; set; }

        [XmlElement("Rolloff")]
        public string Rolloff { get; set; }
    }
}