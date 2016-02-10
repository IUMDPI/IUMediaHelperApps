using System;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace Packager.Models.OutputModels
{
    [Serializable]
    public class ConfigurationData
    {
        [XmlAttribute("type", Form = XmlSchemaForm.Qualified, Namespace = "http://www.w3.org/2001/XMLSchema-instance")]
        public string XsiType { get; set; }
        public string Track { get; set; }
        public string SoundField { get; set; }
        public string Speed { get; set; }
        public string RecordingType { get; set; }
    }
}