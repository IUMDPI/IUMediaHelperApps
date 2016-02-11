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
            FullFileUse = "";
            FileUse = "";
        }
        
        public override bool IsValid()
        {
            return ProjectCode.IsSet() && BarCode.IsSet();
        }
    }
}