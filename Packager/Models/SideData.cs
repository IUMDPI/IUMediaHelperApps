using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace Packager.Models
{
    [Serializable]
    public class SideData
    {
        [XmlAttribute("Side")]
        public string Side { get; set; }
        public IngestData Ingest { get; set; }
        public string ManualCheck { get; set; }
        public List<FileData> Files { get; set; }
    }
}