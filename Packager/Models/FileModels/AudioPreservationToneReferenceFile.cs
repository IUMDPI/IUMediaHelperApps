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

        public override int Precedence => 1; //todo: figure out proper value
    }
}
