namespace Packager.Models.FileModels
{
    public abstract class AbstractPreservationFile : AbstractFile
    {
        protected AbstractPreservationFile(AbstractFile original) : base(original)
        {
        }

        public override string FileUse => "pres";
        public override string FullFileUse => "Preservation Master";
    }
}