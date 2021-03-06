﻿using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using Packager.Extensions;
using Packager.Models.OutputModels.Ingest;

namespace Packager.Models.OutputModels
{
    [Serializable]
    public class SideData
    {
        [XmlAttribute("Side")]
        public string Side { get; set; }

        [XmlElement("Ingest")]
        public List<AbstractIngest> Ingest { get; set; }

        public List<File> Files { get; set; }
    }
}