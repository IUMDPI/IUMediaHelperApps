using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Packager.Models
{
    
    [Serializable]
    [XmlRoot(ElementName = "Auth")]
    public class PodAuth
    {
        [XmlElement(ElementName = "username")]
        public string UserName { get; set; }
        [XmlElement(ElementName = "password")]
        public string Password { get; set; }

    }
}
