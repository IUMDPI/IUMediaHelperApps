using System;
using System.Collections.Generic;
using System.Xml.Linq;
using Packager.Extensions;
using Packager.Factories;
using Packager.Validators.Attributes;

namespace Packager.Models.PodMetadataModels
{
    public abstract class AbstractPodMetadata : BasePodResponse
    {
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

        public List<AbstractDigitalFile> FileProvenances { get; set; }
        public string Comments { get; set; }

        public override void ImportFromXml(XElement element, IImportableFactory factory)
        {
            base.ImportFromXml(element, factory);

            Comments = factory.ToStringValue(element, "data/comments"); // todo: needs to be in new endpoint
            Format = factory.ToStringValue(element, "data/format");
            CallNumber = factory.ToStringValue(element, "data/call_number");
            Title = factory.ToStringValue(element, "data/title");
            Unit = factory.ToStringValue(element, "data/unit");
            Barcode = factory.ToStringValue(element, "data/mdpi_barcode");
            DigitizingEntity = factory.ToStringValue(element, "data/digitizing_entity");
            BakingDate = factory.ToUtcDateTimeValue(element, "data/baking_date");
            CleaningDate = factory.ToUtcDateTimeValue(element, "data/cleaning_date");
            CleaningComment = factory.ToStringValue(element, "data/cleaning_comment");
            Repaired = factory.ToBooleanValue(element, "data/repaired").ToYesNo();
            Damage = factory.ToStringValue(element, "data/damage").ToDefaultIfEmpty("None");
            PreservationProblems = factory.ToStringValue(element, "data/preservation_problems");
            
            // process each digital_file_provenance node
            FileProvenances = ImportFileProvenances(element,
                "data/digital_files/digital_file_provenance",
                factory);

            NormalizeFileProvenances();
        }


        // method is abstract; implemented in derived classes
        protected abstract List<AbstractDigitalFile> ImportFileProvenances(XElement element, string path,
            IImportableFactory factory);

        protected abstract void NormalizeFileProvenances();
    }
}