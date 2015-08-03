using System.Collections.Generic;
using System.Linq;
using System.Text;
using Packager.Extensions;

namespace Packager.Models.PodMetadataModels
{
    public class ConsolidatedPodMetadata
    {
        private const string NotSetText = "[not set]";
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
        public string Damage { get; set; }
        public string PreservationProblems { get; set; }

        public bool IsDigitalFormat()
        {
            return Format.ToLowerInvariant().Equals("cd-r") ||
                   Format.ToLowerInvariant().Equals("dat");
        }

        public override string ToString()
        {
            var builder = new StringBuilder();

            builder.Append(GetStringPropertiesAndValues(this));

            foreach (var provenance in FileProvenances)
            {
                builder.AppendLine();
                builder.AppendFormat("File Provenance: {0}\n", provenance.Filename.ToDefaultIfEmpty(NotSetText));
                builder.Append(GetStringPropertiesAndValues(provenance, "\t"));
            }

            return builder.ToString();
        }

        private static string GetStringPropertiesAndValues(object instance, string indent = "")
        {
            var builder = new StringBuilder();
            foreach (var property in instance.GetType()
                .GetProperties()
                .Where(p => p.PropertyType == typeof (string)))
            {
                builder.AppendFormat("{0}{1}: {2}\n",
                    indent,
                    property.Name.FromCamelCaseToSpaces(),
                    (property.GetValue(instance)).ToDefaultIfEmpty(NotSetText));
            }

            return builder.ToString();
        }
    }
}