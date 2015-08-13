using System;
using System.Xml.Serialization;
using Packager.Validators.Attributes;

namespace Packager.Models
{
    [Serializable]
    [XmlRoot(ElementName = "Auth")]
    public class PodAuth
    {
        [XmlElement(ElementName = "username")]
        [Required]
        public string UserName { get; set; }

        [XmlElement(ElementName = "password")]
        [Required]
        public string Password { get; set; }
    }
}