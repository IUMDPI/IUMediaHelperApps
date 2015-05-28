using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
using Packager.Attributes;

namespace Packager.Models
{
    [Serializable]
    public class PartsData
    {
        [ExcelField("DigitizingEntity", true)]
        public string DigitizingEntity { get; set; }

        [XmlElement("Part")]
        public SideData[] Sides { get; set; }
    }
}
