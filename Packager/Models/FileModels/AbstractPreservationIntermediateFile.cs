using Common.Models;

namespace Packager.Models.FileModels
{
    public abstract class AbstractPreservationIntermediateFile : AbstractFile
    {
        protected AbstractPreservationIntermediateFile(AbstractFile original, string extension) : 
            base(original, FileUsages.PreservationIntermediateMaster, extension)
        {
        }

        public override bool ShouldNormalize => true;
        public override int Precedence => 2;
    }
}