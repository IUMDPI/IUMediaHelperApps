namespace Packager.Models.FileModels
{
    public class MezzanineFile : AbstractFile
    {
        private const string FileUseValue = "mezz";
        private const string FullFileUseValue = "Mezzanine File";
        private const string ExtensionValue = ".mov";

        public MezzanineFile(AbstractFile original) :
            base(original, FileUseValue, FullFileUseValue, ExtensionValue)
        {
        }
    }
}