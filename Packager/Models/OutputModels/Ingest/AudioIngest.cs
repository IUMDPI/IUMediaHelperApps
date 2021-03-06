using System;
using System.Xml.Serialization;

namespace Packager.Models.OutputModels.Ingest
{
    [Serializable]
    public class AudioIngest : AbstractIngest
    {
        [XmlElement(Order = 1)]
        public string FileName { get; set; }

        [XmlElement(Order = 2)]
        public string Date { get; set; }

        [XmlElement(Order = 3)]
        public string Comments { get; set; }

        [XmlElement(ElementName = "Created_by", Order = 4)]
        public string CreatedBy { get; set; }

        [XmlElement(Order = 13, ElementName = "Player")]
        public Device[] Players { get; set; }

        [XmlElement(Order = 14, ElementName = "PreAmp")]
        public Device[] PreAmpDevices { get; set; }

        [XmlElement(Order = 15, ElementName = "AD")]
        public Device[] AdDevices { get; set; }

        [XmlElement(Order = 16, ElementName = "Extraction_workstation")]
        public Device ExtractionWorkstation { get; set; }
        
        [XmlElement("Speed_used", Order = 5)]
        public string SpeedUsed { get; set; }

        [XmlElement("Stylus", Order = 6)]
        public string Stylus { get; set; }

        [XmlElement("Turnover", Order = 7)]
        public string Turnover { get; set; }

        [XmlElement("Rolloff", Order = 8)]
        public string Rolloff { get; set; }

        [XmlElement("Reference_fluxivity", Order = 9)]
        public string ReferenceFluxivity { get; set; }

        [XmlElement("Gain", Order = 10)]
        public string Gain { get; set; }

        [XmlElement("Analog_output_voltage", Order = 11)]
        public string AnalogOutputVoltage { get; set; }

        [XmlElement("Peak", Order = 12)]
        public string Peak { get; set; }

       
    }
}