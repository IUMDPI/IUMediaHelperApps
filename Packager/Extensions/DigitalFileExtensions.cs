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
            var result = provenances.SingleOrDefault(dfp => model.IsSameAs(FileModelFactory.GetModel(dfp.Filename)));
            return result ?? defaultValue;
        }
    }
}