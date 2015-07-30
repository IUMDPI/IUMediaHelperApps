using System.Linq;
using Packager.Models.FileModels;
using Packager.Models.PodMetadataModels;

namespace Packager.Extensions
{
    public static class PodMetadataExtensions
    {
        public static DigitalFileProvenance GetFileProvenance(this DigitalProvenance provenance, AbstractFileModel model, DigitalFileProvenance defaultValue = null)
        {
            var result = provenance.DigitalFileProvenances.SingleOrDefault(dfp => model.IsSameAs(dfp.Filename));
            return result ?? defaultValue;
        }
    }
}