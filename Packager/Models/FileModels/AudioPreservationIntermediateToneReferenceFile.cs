using Common.Models;

namespace Packager.Models.FileModels
{
    public class AudioPreservationIntermediateToneReferenceFile : AbstractFile
    {
        private const string ExtensionValue = ".wav";

        public AudioPreservationIntermediateToneReferenceFile(AbstractFile original) : 
            base(original, FileUsages.PreservationIntermediateToneReference, ExtensionValue)
        {
        }

        public override int Precedence => 3; //todo: figure out proper value
    }
}