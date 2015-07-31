using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Security.Policy;
using System.Text;

namespace Packager.Models.PodMetadataModels
{
    public class ConsolidatedPodMetadata
    {

        public string Identifier { get; set; }
        public string Format { get; set; }
        public string CallNumber { get; set; }
        public string Title { get; set; }
        public string Unit { get; set; }
        public string Barcode { get; set; }
        public string Brand { get; set; }
        public string DirectionsRecorded { get; set; }
        public string CleaningDate { get; set; }
        public string CleaningComment { get; set; }
        public string BakingDate { get; set; }
        public string Repaired { get; set; }
        public string DigitizingEntity { get; set; }
        
        public string PlaybackSpeed { get; set; }
        public string TrackConfiguration { get; set; }
        public string SoundField { get; set; }
        public string TapeThickness { get; set; }

        public List<DigitalFileProvenance> FileProvenances { get; set; }
        

        public bool IsDigitalFormat()
        {
            return Format.ToLowerInvariant().Equals("cd-r") ||
                   Format.ToLowerInvariant().Equals("dat");
        }




























        

        
        

        
        

        private static string GetBoolValuesAsList(object instance)
        {
            if (instance == null)
            {
                return string.Empty;
            }

            var properties = instance.GetType().GetProperties().Where(p => p.PropertyType == typeof(bool));
            var results = (properties.Where(property => (bool)property.GetValue(instance))
                .Select(GetNameOrDescription)).Distinct().ToList();
            return string.Join(", ", results);
        }

        private static string GetNameOrDescription(MemberInfo propertyInfo)
        {
            var description = propertyInfo.GetCustomAttribute<DescriptionAttribute>();
            return description == null ? propertyInfo.Name : description.Description;
        }
    }
}