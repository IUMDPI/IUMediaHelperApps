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

            Identifier = factory.ToStringValue(element,"data/object/details/id");
            Format = factory.ToStringValue(element,"data/object/details/format");
            CallNumber = factory.ToStringValue(element,"data/object/details/call_number");
            Title = factory.ToStringValue(element,"data/object/details/title");
            Unit = factory.ToStringValue(element,"data/object/assignment/unit");
            Barcode = factory.ToStringValue(element,"data/object/details/mdpi_barcode");

            CleaningDate = factory.ToUtcDateTimeValue(element,"data/object/digital_provenance/cleaning_date");
            CleaningComment = factory.ToStringValue(element,"data/object/digital_provenance/cleaning_comment");
            BakingDate = factory.ToUtcDateTimeValue(element,"data/object/digital_provenance/baking_date");
            Repaired = factory.ToBooleanValue(element,"data/object/digital_provenance/repaired").ToYesNo();
            Damage = factory.ResolveDamage(element, "data/object/technical_metadata/damage").ToDefaultIfEmpty("None");
            PreservationProblems = factory.ResolvePreservationProblems(element, "data/object/technical_metadata/preservation_problems");
            DigitizingEntity = factory.ToStringValue(element,"data/object/digital_provenance/digitizing_entity");

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