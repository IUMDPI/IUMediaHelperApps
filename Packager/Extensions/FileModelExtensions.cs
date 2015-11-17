﻿using System.Collections.Generic;
using System.Linq;
using Packager.Models.FileModels;

namespace Packager.Extensions
{
    public static class FileModelExtensions
    {
        public static bool IsObjectModel(this AbstractFileModel fileModel)
        {
            return fileModel is ObjectFileModel;
        }

        public static bool IsXmlModel(this AbstractFileModel fileModel)
        {
            return fileModel is XmlFileModel;
        }

        public static bool IsUnknownModel(this AbstractFileModel fileModel)
        {
            return fileModel is UnknownFileModel;
        }
        
        public static ObjectFileModel GetPreservationOrIntermediateModel<T>(this IGrouping<T, ObjectFileModel> grouping)
        {
            var preservationIntermediate = grouping.SingleOrDefault(m => m.IsPreservationIntermediateVersion());
            return preservationIntermediate ?? grouping.SingleOrDefault(m => m.IsPreservationVersion());
        }
        
        public static ObjectFileModel GetPreservationOrIntermediateModel(this IEnumerable<ObjectFileModel> models)
        {
            var list = models.ToList();

            var preservationIntermediate = list.FirstOrDefault(m => m.IsPreservationIntermediateVersion());
            return preservationIntermediate ?? list.FirstOrDefault(m => m.IsPreservationVersion());
        }

        public static List<ObjectFileModel> RemoveDuplicates(this List<ObjectFileModel> models)
        {
            return models.GroupBy(o => o.ToFileName())
                .Select(g => g.First()).ToList();
        }
    }
}