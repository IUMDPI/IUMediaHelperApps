using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using Packager.Validators.Attributes;

namespace Packager.Models.PodMetadataModels.ConsolidatedModels
{
    public abstract class AbstractConsolidatedPodMetadata
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

        public List<DigitalFileProvenance> FileProvenances { get; set; }
        public string Damage { get; set; }
        public string PreservationProblems { get; set; }

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
            
            FileProvenances = metadata.Data.Object.DigitalProvenance.DigitalFiles;
            Damage = GetBoolValuesAsList(metadata.Data.Object.TechnicalMetadata.Damage, "None");
            PreservationProblems = GetBoolValuesAsList(metadata.Data.Object.TechnicalMetadata.PreservationProblems);
        }

        protected static string GetBoolValuesAsList(object instance, string defaultValue = "")
        {
            if (instance == null)
            {
                return defaultValue;
            }

            var properties = instance.GetType().GetProperties().Where(p => p.PropertyType == typeof(bool));
            var results = (properties.Where(property => (bool)property.GetValue(instance))
                .Select(GetNameOrDescription)).Distinct().ToList();

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
    }
}