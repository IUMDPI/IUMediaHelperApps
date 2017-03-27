using Common.Models;

namespace Packager.Models.FileModels
{
    public class TiffImageFile:AbstractFile
    {
        private const string ExtensionValue = ".tif";

        public TiffImageFile(AbstractFile original) : base(original, FileUsages.LabelImageFile, ExtensionValue)
        {
        }

        public override int Precedence => 4;
    }
}
