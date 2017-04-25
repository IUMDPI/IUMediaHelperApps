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

        public override int Precedence => 4; //todo: figure out proper value
    }

    public class AudioPreservationIntermediateToneReferenceFile : AbstractFile
    {
        private const string ExtensionValue = ".wav";

        public AudioPreservationIntermediateToneReferenceFile(AbstractFile original) : 
            base(original, FileUsages.PreservationIntermediateToneReference, ExtensionValue)
        {
        }

        public override int Precedence => 4; //todo: figure out proper value
    }
}
