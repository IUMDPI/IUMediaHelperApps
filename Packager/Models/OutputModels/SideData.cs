using System;
using System.Collections.Generic;
using System.Threading.Tasks;
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

        public List<File> Files { get; set; }
    }
}