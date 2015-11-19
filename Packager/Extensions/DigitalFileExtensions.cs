using System.Collections.Generic;
using System.Linq;
using Packager.Models.FileModels;
using Packager.Models.PodMetadataModels;
using System.IO;

namespace Packager.Extensions
{
    public static class DigitalFileExtensionss
    {
        public static AbstractDigitalFile GetFileProvenance(this IEnumerable<AbstractDigitalFile> provenances, AbstractFileModel model, AbstractDigitalFile defaultValue = null)
        {
            var result = provenances.SingleOrDefault(dfp => model.IsSameAs(NormalizeFilename(dfp.Filename, model)));
            return result ?? defaultValue;
        }

        private static string NormalizeFilename(string value, AbstractFileModel model)
        {
            return Path.ChangeExtension(value, model.Extension);
        }
    }
}