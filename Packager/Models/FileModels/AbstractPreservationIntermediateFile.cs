namespace Packager.Models.FileModels
{
    public abstract class AbstractPreservationIntermediateFile : AbstractFile
    {
        private const string FileUseValue = "presInt";
        private const string FullFileUseValue = "Preservation Master - Intermediate";

        protected AbstractPreservationIntermediateFile(AbstractFile original, string extension) : 
            base(original, FileUseValue, FullFileUseValue, extension)
        {
        }

    }
}