using System;
using System.Xml.Serialization;

namespace Packager.Models.OutputModels
{
    [Serializable]
    public class AudioCarrierData:AbstractCarrierData
    {
        // todo: figure out where to get this from

        [XmlElement(Order = 3)]
        public ConfigurationData Configuration { get; set; }

        [XmlElement(Order = 4)]
        public string Brand { get; set; }

        [XmlElement(Order = 5)]
        public string Thickness { get; set; }

        [XmlElement(Order = 6)]
        public string DirectionsRecorded { get; set; }

        [XmlElement(Order = 8)]
        public string Repaired { get; set; }
    }
}