using System.ComponentModel;

namespace Packager.Models.PodMetadataModels
{
    public class PreservationProblems
    {
        [Description("Breakdown of materials")]
        public bool BreakdownOfMaterials { get; set; }

        [Description("Delamination")]
        public bool Delamination { get; set; }

        [Description("Exudation")]
        public bool Exudation { get; set; }

        [Description("Fungus")]
        public bool Fungus { get; set; }

        [Description("Other contaminants")]
        public bool OtherContaminants { get; set; }

        [Description("Oxidation")]
        public bool Oxidation { get; set; }

        [Description("Soft binder syndrome")]
        public bool SoftBinderSyndrome { get; set; }

        [Description("Vinegar syndrome")]
        public bool VinegarSyndrome { get; set; }
    }
}