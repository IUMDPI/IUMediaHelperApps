using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;
using System.Xml.XPath;
using Packager.Deserializers;
using Packager.Extensions;
using Packager.Validators.Attributes;

namespace Packager.Models.PodMetadataModels.ConsolidatedModels
{
    public abstract class AbstractConsolidatedPodMetadata : IImportable
    {
        private static readonly Dictionary<string, string> KnownPreservationProblems = new Dictionary<string, string>
        {
            {"breakdown_of_materials", "Breakdown of materials"},
            {"delamination", "Delamination"},
            {"exudation", "Exudation"},
            {"fungus", "Fungus"},
            {"other_contaminants", "Other contaiminants"},
            {"oxidation", "Oxidation"},
            {"soft_binder_syndrome", "Soft binder syndrome"},
            {"vinegar_syndrome", "Vinegar syndrome"}
        };

        private static readonly Dictionary<string, string> KnownDamageValues = new Dictionary<string, string>
        {
            {"broken", "Broken"},
            {"cracked", "Cracked"},
            {"dirty", "Dirty"},
            {"fungus", "Fungus"},
            {"scratched", "Scratched"},
            {"warped", "Warped"},
            {"worn", "Worn"}
        };


        public bool Success { get; set; }

        public string Message { get; set; }

        [Required]
        public string Identifier { get; set; }

        [Required]
        public string Format { get; set; }

        [Required]
        public string CallNumber { get; set; }

        public string Title { get; set; }

        [Required]
        public string Unit { get; set; }

        [Required]
        public string Barcode { get; set; }

        public DateTime? CleaningDate { get; set; }
        public string CleaningComment { get; set; }
        public DateTime? BakingDate { get; set; }
        public string Repaired { get; set; }

        [Required]
        public string DigitizingEntity { get; set; }

        public string Damage { get; set; }
        public string PreservationProblems { get; set; }

        public List<AbstractConsolidatedDigitalFile> FileProvenances { get; set; }

        public virtual void ImportFromXml(XElement element)
        {
            Success = element.ToBooleanValue("success");
            Message = element.ToStringValue("message");
            Identifier = element.ToStringValue("data/object/details/id");
            Format = element.ToStringValue("data/object/details/format");
            CallNumber = element.ToStringValue("data/object/details/call_number");
            Title = element.ToStringValue("data/object/details/title");
            Unit = element.ToStringValue("data/object/assignment/unit");
            Barcode = element.ToStringValue("data/object/details/mdpi_barcode");

            CleaningDate = element.ToDateTimeValue("data/object/digital_provenance/cleaning_date");
            CleaningComment = element.ToStringValue("data/object/digital_provenance/cleaning_comment");
            BakingDate = element.ToDateTimeValue("data/object/digital_provenance/baking_date");
            Repaired = element.ToBooleanValue("data/object/digital_provenance/repaired").ToYesNo();
            Damage = element.ToResolvedDelimitedString("data/object/technical_metadata/damage", KnownDamageValues).ToDefaultIfEmpty("None");
            PreservationProblems = element.ToResolvedDelimitedString("data/object/technical_metadata/preservation_problems", KnownPreservationProblems);
            DigitizingEntity = element.ToStringValue("data/object/digital_provenance/digitizing_entity");
            FileProvenances = ImportFileProvenances(element.XPathSelectElements("data/object/digital_provenance/digital_files/digital_file_provenance"));
        }
        
        protected abstract List<AbstractConsolidatedDigitalFile> ImportFileProvenances(IEnumerable<XElement> elements);
    }
}