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
            if (string.IsNullOrWhiteSpace(ProjectCode))
            {
                return false;
            }

            return !string.IsNullOrWhiteSpace(BarCode);
        }
    }
}