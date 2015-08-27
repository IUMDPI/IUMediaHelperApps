using System;
using System.Xml.Serialization;

namespace Packager.Models.OutputModels
{
    [Serializable]
    public class FileData
    {
        public string FileName { get; set; }

        [XmlElement("CheckSum")]
        public string Checksum { get; set; }
    }
}