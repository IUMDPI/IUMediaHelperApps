using System.Collections.Generic;
using System.Linq;
using Packager.Models.FileModels;
using Packager.Models.PodMetadataModels;

namespace Packager.Extensions
{
    public static class PodMetadataExtensions
    {
        public static DigitalFileProvenance GetFileProvenance(this List<DigitalFileProvenance> provenances, AbstractFileModel model, DigitalFileProvenance defaultValue = null)
        {
            var result = provenances.SingleOrDefault(dfp => model.IsSameAs(dfp.Filename));
            return result ?? defaultValue;
        }
    }
}