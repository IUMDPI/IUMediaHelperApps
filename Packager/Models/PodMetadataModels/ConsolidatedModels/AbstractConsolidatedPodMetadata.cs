using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;
using Packager.Deserializers;
using Packager.Extensions;
using Packager.Validators.Attributes;

namespace Packager.Models.PodMetadataModels.ConsolidatedModels
{
    public abstract class AbstractConsolidatedPodMetadata:IImportableFromPod
    {
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

        public virtual void ImportFromFullMetadata(PodMetadata metadata)
        {
            Identifier = metadata.Data.Object.Details.Id;
            Format = metadata.Data.Object.Details.Format;
            CallNumber = metadata.Data.Object.Details.CallNumber;
            Title = metadata.Data.Object.Details.Title;
            Unit = metadata.Data.Object.Assignment.Unit;
            Barcode = metadata.Data.Object.Details.MdpiBarcode;

            CleaningDate = metadata.Data.Object.DigitalProvenance.CleaningDate;
            CleaningComment = metadata.Data.Object.DigitalProvenance.CleaningComment;
            BakingDate = metadata.Data.Object.DigitalProvenance.Baking;
            Repaired = ToYesNo(metadata.Data.Object.DigitalProvenance.Repaired);
            DigitizingEntity = metadata.Data.Object.DigitalProvenance.DigitizingEntity;
            Damage = GetBoolValuesAsList(metadata.Data.Object.TechnicalMetadata.Damage, "None");
            PreservationProblems = GetBoolValuesAsList(metadata.Data.Object.TechnicalMetadata.PreservationProblems);
            FileProvenances = ImportFileProvenances(metadata.Data.Object.DigitalProvenance.DigitalFiles);
        }

        public virtual void ImportFromXml(XDocument document)
        {
            Success = document.GetValue("/pod/success", false);
            Message = document.GetValue("/pod/message", string.Empty);
            Identifier = document.GetValue("/pod/data/object/details/id", string.Empty);
            Format = document.GetValue("/pod/data/object/details/format", string.Empty);
            CallNumber = document.GetValue("/pod/data/object/details/call_number", string.Empty);
            Title = document.GetValue("/pod/data/object/details/title", string.Empty);
            Unit = document.GetValue("/pod/data/object/assignment/unit", string.Empty);

            CleaningDate = document.GetValue("/pod/data/object/digital_provenance/cleaning_date", (DateTime?) null);
            CleaningComment = document.GetValue("/pod/data/object/digital_provenance/cleaning_comment", string.Empty);
            BakingDate = document.GetValue("/pod/data/object/digital_provenance/baking_date", (DateTime?)null);
            Repaired = ToYesNo(document.GetValue("/pod/data/object/digital_provenance/repaired", false));
            Damage = document.BoolValuesToString("/pod/data/object/technical_metadata/damage", "None");
            PreservationProblems = document.BoolValuesToString("/pod/data/object/technical_metadata/preservation_problems", string.Empty);
            FileProvenances = ImportFileProvenances(document);
        }

        public void ImportFromXml(XElement element)
        {
            throw new NotImplementedException();
        }

        protected static string GetBoolValuesAsList(object instance, string defaultValue = "")
        {
            if (instance == null)
            {
                return defaultValue;
            }

            var properties = instance.GetType().GetProperties().Where(p => p.PropertyType == typeof (bool));
            var results = properties.Where(property => (bool) property.GetValue(instance))
                .Select(GetNameOrDescription).Distinct().ToList();

            return results.Any()
                ? string.Join(", ", results)
                : defaultValue;
        }

        private static string GetNameOrDescription(MemberInfo propertyInfo)
        {
            var description = propertyInfo.GetCustomAttribute<DescriptionAttribute>();
            return description == null ? propertyInfo.Name : description.Description;
        }

        private static string ToYesNo(bool? value)
        {
            if (value.HasValue == false)
            {
                return "No";
            }
            return value.Value ? "Yes" : "No";
        }

        protected abstract List<AbstractConsolidatedDigitalFile> ImportFileProvenances(
            IEnumerable<DigitalFileProvenance> originals);

        protected abstract List<AbstractConsolidatedDigitalFile> ImportFileProvenances(XDocument document);
    }
}