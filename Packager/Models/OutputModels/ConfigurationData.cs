using System;
using System.Xml.Serialization;

namespace Packager.Models.OutputModels
{
    [Serializable]
    public class ConfigurationData
    {
        [XmlAttribute(AttributeName = "xsi:type")]
        public string XsiType { get; set; }
        
        public string Track { get; set; }
        public string SoundField { get; set; }
        public string Speed { get; set; }
        public string RecordingType { get; set; }
    }
}