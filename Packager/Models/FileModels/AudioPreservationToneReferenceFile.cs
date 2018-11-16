using Common.Models;

namespace Packager.Models.FileModels
{
    public class AudioPreservationToneReferenceFile : AbstractFile
    {
        private const string ExtensionValue = ".wav";

        public AudioPreservationToneReferenceFile(AbstractFile original) : 
            base(original, FileUsages.PreservationToneReference, ExtensionValue)
        {
        }

        public override bool ShouldNormalize => true;

        public override int Precedence => 1;
    }
}
