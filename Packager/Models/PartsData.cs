using System;
using System.Xml.Serialization;

namespace Packager.Models
{
    [Serializable]
    public class PartsData
    {
        public string DigitizingEntity { get; set; }

        [XmlElement("Part")]
        public SideData[] Sides { get; set; }
    }
}