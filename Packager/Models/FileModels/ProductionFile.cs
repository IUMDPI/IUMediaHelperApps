using Common.Models;

namespace Packager.Models.FileModels
{
    public class ProductionFile : AbstractFile
    {
        private const string ExtensionValue = ".wav";

        public ProductionFile(AbstractFile original) : 
            base(original, FileUsages.ProductionMaster, ExtensionValue)
        {
        }
    }
}