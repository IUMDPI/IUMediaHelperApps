namespace Packager.Models.FileModels
{
    public abstract class AbstractPreservationFile : AbstractFile
    {
        private const string FileUseValue = "pres";
        private const string FullFileUseValue = "Preservation Master";

        protected AbstractPreservationFile(AbstractFile original, string extension) : 
            base(original, FileUseValue, FullFileUseValue, extension)
        {
        }
    }
}