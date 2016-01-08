using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using Packager.Models.OutputModels.Ingest;

namespace Packager.Models.OutputModels
{
    [Serializable]
    public class SideData
    {
        [XmlAttribute("Side")]
        public string Side { get; set; }

        public AbstractIngest Ingest { get; set; }
        public string ManualCheck { get; set; }

        public string QCStatus { get; set; }

        public List<File> Files { get; set; }

        public bool ShouldSerializeQCStatus()
        {
            return string.IsNullOrWhiteSpace(QCStatus) == false;
        }

        public bool ShouldSerializeManualCheck()
        {
            return string.IsNullOrWhiteSpace(ManualCheck) == false;
        }
    }
}