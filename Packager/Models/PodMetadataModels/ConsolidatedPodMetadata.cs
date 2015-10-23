using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Packager.Exceptions;
using Packager.Extensions;
using Packager.Models.FileModels;
using Packager.Validators.Attributes;

namespace Packager.Models.PodMetadataModels
{
    public class ConsolidatedPodMetadata
    {
        private const string NotSetText = "[not set]";

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

        public string Brand { get; set; }
        public string DirectionsRecorded { get; set; }
        public DateTime? CleaningDate { get; set; }
        public string CleaningComment { get; set; }
        public DateTime? BakingDate { get; set; }
        public string Repaired { get; set; }

        [Required]
        public string DigitizingEntity { get; set; }

        public string PlaybackSpeed { get; set; }

        public string TrackConfiguration { get; set; }

        [Required]
        public string SoundField { get; set; }

        public string TapeThickness { get; set; }
        public List<DigitalFileProvenance> FileProvenances { get; set; }
        public string Damage { get; set; }
        public string PreservationProblems { get; set; }

        public DigitalFileProvenance GetProvenance(IEnumerable<ObjectFileModel> instances, ObjectFileModel model)
        {
            var sequenceInstances = instances.Where(m => m.SequenceIndicator.Equals(model.SequenceIndicator));
            var sequenceMaster = sequenceInstances.GetPreservationOrIntermediateModel();
            if (sequenceMaster == null)
            {
                throw new BextMetadataException("No corresponding preservation or preservation-intermediate master present for {0}", model.ToFileName());
            }

            var defaultProvenance = FileProvenances.GetFileProvenance(sequenceMaster);
            if (defaultProvenance == null)
            {
                throw new BextMetadataException("No digital file provenance in metadata for {0}", sequenceMaster.ToFileName());
            }

            return GetProvenance(model, defaultProvenance);
        }

        private DigitalFileProvenance GetProvenance(AbstractFileModel model, DigitalFileProvenance defaultValue = null)
        {
            var result = FileProvenances.SingleOrDefault(dfp => model.IsSameAs(NormalizeFilename(dfp.Filename, model)));
            return result ?? defaultValue;
        }

        private static string NormalizeFilename(string value, AbstractFileModel model)
        {
            return Path.ChangeExtension(value, model.Extension);
        }
    }
}