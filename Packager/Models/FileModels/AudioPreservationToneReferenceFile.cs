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

        //todo: figure out proper value
    }
}
