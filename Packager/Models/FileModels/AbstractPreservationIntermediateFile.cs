namespace Packager.Models.FileModels
{
    public abstract class AbstractPreservationIntermediateFile : AbstractFile
    {
        protected AbstractPreservationIntermediateFile(AbstractFile original) : base(original)
        {
        }

        public override string FileUse => "presInt";
        public override string FullFileUse => "Preservation Master - Intermediate";
    }
}