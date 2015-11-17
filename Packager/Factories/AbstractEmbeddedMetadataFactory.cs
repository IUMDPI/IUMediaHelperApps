using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Packager.Exceptions;
using Packager.Extensions;
using Packager.Models.EmbeddedMetadataModels;
using Packager.Models.FileModels;
using Packager.Models.PodMetadataModels;

namespace Packager.Factories
{
    public abstract class AbstractEmbeddedMetadataFactory<T> : IEmbeddedMetadataFactory<T> where T : AbstractPodMetadata
    {
        public AbstractEmbeddedMetadata Generate(IEnumerable<ObjectFileModel> models, ObjectFileModel target, T metadata)
        {
            var provenance = GetProvenance(metadata, models, target);
            return Generate(target, provenance, metadata);
        }

        protected abstract AbstractEmbeddedMetadata Generate(ObjectFileModel model, AbstractDigitalFile provenance, T metadata);

        protected static string GenerateDescription(AbstractPodMetadata metadata, ObjectFileModel model)
        {
            return $"{metadata.Unit}. {metadata.CallNumber}. File Use: {model.FullFileUse}. Original Filename: {Path.GetFileNameWithoutExtension(model.OriginalFileName)}";
        }

        protected static string GetDateString(DateTime? date, string format, string defaultValue)
        {
            return date.HasValue == false ? defaultValue : date.Value.ToString(format);
        }

        private static AbstractDigitalFile GetProvenance(AbstractPodMetadata podMetadata, IEnumerable<ObjectFileModel> instances, ObjectFileModel model)
        {
            var sequenceInstances = instances.Where(m => m.SequenceIndicator.Equals(model.SequenceIndicator));
            var sequenceMaster = sequenceInstances.GetPreservationOrIntermediateModel();
            if (sequenceMaster == null)
            {
                throw new EmbeddedMetadataException("No corresponding preservation or preservation-intermediate master present for {0}", model.ToFileName());
            }

            var defaultProvenance = GetProvenance(podMetadata, sequenceMaster);
            if (defaultProvenance == null)
            {
                throw new EmbeddedMetadataException("No digital file provenance in metadata for {0}", sequenceMaster.ToFileName());
            }

            return GetProvenance(podMetadata, model, defaultProvenance);
        }

        private static AbstractDigitalFile GetProvenance(AbstractPodMetadata podMetadata, AbstractFileModel model, AbstractDigitalFile defaultValue = null)
        {
            var result = podMetadata.FileProvenances.SingleOrDefault(dfp => model.IsSameAs(NormalizeFilename(dfp.Filename, model)));
            return result ?? defaultValue;
        }

        private static string NormalizeFilename(string value, AbstractFileModel model)
        {
            return Path.ChangeExtension(value, model.Extension);
        }
    }
}