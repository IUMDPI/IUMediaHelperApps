using System;
using System.Collections.Generic;
using System.Xml.Linq;
using System.Xml.XPath;
using Packager.Extensions;
using Packager.Factories;
using Packager.Validators.Attributes;

namespace Packager.Models.PodMetadataModels
{
    public abstract class AbstractPodMetadata : BasePodResponse
    {
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

        public List<AbstractDigitalFile> FileProvenances { get; set; }

        public override void ImportFromXml(XElement element, IImportableFactory factory)
        {
            base.ImportFromXml(element, factory);

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
            Damage = factory.ResolveDamage(element, "data/object/technical_metadata/damage").ToDefaultIfEmpty("None");
            PreservationProblems = factory.ResolvePreservationProblems(element, "data/object/technical_metadata/preservation_problems");
            DigitizingEntity = element.ToStringValue("data/object/digital_provenance/digitizing_entity");

            // process each digital_file_provenance node
            FileProvenances = ImportFileProvenances(
                element.XPathSelectElements("data/object/digital_provenance/digital_files/digital_file_provenance"),
                factory);
            NormalizeFileProvenances();
        }

        // method is abstract; implemented in derived classes
        protected abstract List<AbstractDigitalFile> ImportFileProvenances(IEnumerable<XElement> elements,
            IImportableFactory factory);

        protected abstract void NormalizeFileProvenances();
    }
}