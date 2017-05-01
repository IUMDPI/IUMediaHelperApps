using Common.Models;
using Packager.Extensions;

namespace Packager.Models.FileModels
{
    public class XmlFile : AbstractFile
    {
        public XmlFile(string projectCode, string barCode)
        {
            ProjectCode = projectCode;
            BarCode = barCode;
            Extension = ".xml";
            Filename = $"{ProjectCode}_{BarCode}{Extension}";
            FileUsage = FileUsages.XmlFile;
        }

        public override bool IsImportable()
        {
            return ProjectCode.IsSet() && BarCode.IsSet();
        }
    }
}