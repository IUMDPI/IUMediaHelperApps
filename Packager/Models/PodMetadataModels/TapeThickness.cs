using System.ComponentModel;

namespace Packager.Models.PodMetadataModels
{
    public class TapeThickness
    {
        
        [Description(".5")]
        public bool ZeroPoint5Mils { get; set; }

        [Description("1.0")]
        public bool OneMils { get; set; }

        [Description("1.5")]
        public bool OnePoint5Mils { get; set; }
    }
}