using System.Collections.Generic;
using System.Linq;
using Packager.Models.FileModels;
using Packager.Models.PodMetadataModels;
using System.IO;
using Packager.Factories;

namespace Packager.Extensions
{
    public static class DigitalFileExtensions
    {
        public static AbstractDigitalFile GetFileProvenance(this IEnumerable<AbstractDigitalFile> provenances, AbstractFile model, AbstractDigitalFile defaultValue = null)
        {
            var result = provenances.SingleOrDefault(dfp => model.IsSameAs(NormalizeFileNameAndGetModel(model, dfp.Filename)));
            return result ?? defaultValue;
        }

        // in some cases, filenames might be stored in the POD without extensions
        // this method ensures that filenames have extension
        private static AbstractFile NormalizeFileNameAndGetModel(AbstractFile model, string value)
        {
            var normalizedValue = Path.HasExtension(value)? value : $"{value}{model.Extension}";
            return FileModelFactory.GetModel(normalizedValue);

        }
    }
}