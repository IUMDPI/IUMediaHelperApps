namespace Packager.Models.FileModels
{
    public class ProductionFile : AbstractFile
    {
        private const string FileUseValue = "prod";
        private const string FullFileUseValue = "Production Master";
        private const string ExtensionValue = ".wav";

        public ProductionFile(AbstractFile original) : 
            base(original, FileUseValue, FullFileUseValue, ExtensionValue)
        {
        }
    }
}