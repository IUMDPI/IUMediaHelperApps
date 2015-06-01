using System.Linq;

namespace Packager.Models.FileModels
{
    public class ExcelFileModel:AbstractFileModel
    {
        public ExcelFileModel()
        {
        }

        public ExcelFileModel(string path) : base(path)
        {
        }

        public override string ToFileName()
        {
            var parts = new[] { ProjectCode, BarCode};

            var fileName = string.Join("_", parts.Where(p => !string.IsNullOrWhiteSpace(p)));
            return string.Format("{0}{1}", fileName, Extension);
        }
    }
}
