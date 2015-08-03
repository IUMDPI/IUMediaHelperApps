using System.Collections.Generic;

namespace Packager.Models.PodMetadataModels
{
    public class DigitalProvenance
    {
        public string DigitizingEntity { get; set; }
        public string Comments { get; set; }
        public string CleaningDate { get; set; }
        public string Baking { get; set; }
        public bool Repaired { get; set; }
        public string CleaningComment { get; set; }
        public string Duration { get; set; }
        public List<DigitalFileProvenance> DigitalFileProvenances { get; set; }
    }
}