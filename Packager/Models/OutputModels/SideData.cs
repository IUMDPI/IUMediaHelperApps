using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace Packager.Models.OutputModels
{
    [Serializable]
    public class SideData
    {
        [XmlAttribute("Side")]
        public string Side { get; set; }
        public IngestData Ingest { get; set; }
        public string ManualCheck { get; set; }

        [XmlElement("File")]
        public List<FileData> Files { get; set; }
    }
}