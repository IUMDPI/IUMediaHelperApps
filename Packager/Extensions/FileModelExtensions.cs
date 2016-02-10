using System.Collections.Generic;
using System.Linq;
using Packager.Models.FileModels;

namespace Packager.Extensions
{
    public static class FileModelExtensions
    {
        public static bool IsXmlModel(this AbstractFile fileModel)
        {
            return fileModel is XmlFile;
        }

        public static AbstractFile GetPreservationOrIntermediateModel(
            this IEnumerable<AbstractFile> models)
        {
            var list = models.Where(m => !(m is QualityControlFile)).ToList();

            var preservationIntermediate = list.FirstOrDefault(m => m.IsPreservationIntermediateVersion());
            return preservationIntermediate ?? list.FirstOrDefault(m => m.IsPreservationVersion());
        }

        public static List<AbstractFile> RemoveDuplicates(this IEnumerable<AbstractFile> models)
        {
            return models.GroupBy(o => o.Filename)
                .Select(g => g.First()).ToList();
        }

        public static bool IsPreservationVersion(this AbstractFile model)
        {
            return model is AbstractPreservationFile;
        }

        public static bool IsPreservationIntermediateVersion(this AbstractFile model)
        {
            return model is AbstractPreservationIntermediateFile;
        }

        public static bool IsProductionVersion(this AbstractFile model)
        {
            return model is ProductionFile;
        }

        public static bool IsMezzanineVersion(this AbstractFile model)
        {
            return model is MezzanineFile;
        }

        public static bool IsAccessVersion(this AbstractFile model)
        {
            return model is AccessFile;
        }
    }
}