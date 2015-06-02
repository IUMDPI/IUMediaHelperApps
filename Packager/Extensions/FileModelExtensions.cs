using Packager.Models.FileModels;

namespace Packager.Extensions
{
    public static class FileModelExtensions
    {
        public static bool IsExcelModel(this AbstractFileModel fileModel)
        {
            return fileModel is ExcelFileModel;
        }

        public static bool IsArtifactModel(this AbstractFileModel fileModel)
        {
            return fileModel is ArtifactFileModel;
        }

        public static bool IsXmlModel(this AbstractFileModel fileModel)
        {
            return fileModel is XmlFileModel;
        }

        public static bool IsUnknownModel(this AbstractFileModel fileModel)
        {
            return fileModel is UnknownFileModel;
        }
    }
}