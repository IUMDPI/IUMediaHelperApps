using System;
using Packager.Attributes;

namespace Packager.Models
{
    [Serializable]
    public class ConfigurationData
    {
        [ExcelField("Configuration-Track", true)]
        public string Track { get; set; }

        [ExcelField("Configuration-SoundField", true)]
        public string SoundField { get; set; }

        [ExcelField("Configuration-Speed", true)]
        public string Speed { get; set; }
    }
}