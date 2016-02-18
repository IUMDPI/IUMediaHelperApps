using System;
using System.Xml.Serialization;

namespace Packager.Models.OutputModels.Ingest
{
    [Serializable]
    public class AudioIngest : AbstractIngest
    {
        [XmlElement(Order = 1)]
        public string Date { get; set; }

        [XmlElement(Order = 2)]
        public string Comments { get; set; }

        [XmlElement(ElementName = "Created_by", Order = 3)]
        public string CreatedBy { get; set; }

        [XmlElement(Order = 14)]
        public Device[] Players { get; set; }

        [XmlElement(Order = 15)]
        public Device[] AdDevices { get; set; }

        [XmlElement(Order = 16)]
        public Device ExtractionWorkstation { get; set; }
        
        [XmlElement("Speed_used", Order = 4)]
        public string SpeedUsed { get; set; }

        [XmlElement("Preamp", Order = 5)]
        public string PreAmp { get; set; }

        [XmlElement("Preamp_serial_number", Order = 6)]
        public string PreAmpSerialNumber { get; set; }

        [XmlElement("Stylus", Order = 7)]
        public string Stylus { get; set; }

        [XmlElement("Turnover", Order = 8)]
        public string Turnover { get; set; }

        [XmlElement("Rolloff", Order = 9)]
        public string Rolloff { get; set; }

        [XmlElement("Reference_fluxivity", Order = 10)]
        public string ReferenceFluxivity { get; set; }

        [XmlElement("Gain", Order = 11)]
        public string Gain { get; set; }

        [XmlElement("Analog_output_voltage", Order = 12)]
        public string AnalogOutputVoltage { get; set; }

        [XmlElement("Peak", Order = 13)]
        public string Peak { get; set; }

       
    }
}