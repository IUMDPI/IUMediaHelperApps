using System.IO;
using System.Linq;

namespace Packager.Models.FileModels
{
    public class XmlFileModel : AbstractFileModel
    {
        public XmlFileModel()
        {
        }

        public XmlFileModel(string path) : base(path)
        {
        }

        public override bool IsSameAs(string filename)
        {
            var model = new XmlFileModel(Path.GetFileName(filename));
            if (model.ProjectCode != ProjectCode)
            {
                return false;
            }

            if (model.BarCode != BarCode)
            {
                return false;
            }

            return model.Extension.Equals(Extension);
        }

        public override string ToFileName()
        {
            var parts = new[] {ProjectCode, BarCode};

            var fileName = string.Join("_", parts.Where(p => !string.IsNullOrWhiteSpace(p)));
            return string.Format("{0}{1}", fileName, Extension);
        }
    }
}