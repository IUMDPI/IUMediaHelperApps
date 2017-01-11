namespace Packager.Models.FileModels
{
    public class TiffImageFile:AbstractFile
    {
        private const string FileUseValue = "label";
        private const string FullFileUseValue = "Label Image File";
        private const string ExtensionValue = ".tif";

        public TiffImageFile(AbstractFile original) : base(original, FileUseValue, FullFileUseValue, ExtensionValue)
        {
        }

        public override int Precedence => 4;
    }
}
