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

        //todo: figure out proper value
    }
}