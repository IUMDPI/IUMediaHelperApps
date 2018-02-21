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

        public override bool ShouldNormalize => true;
        public override int Precedence => 4;
    }
}