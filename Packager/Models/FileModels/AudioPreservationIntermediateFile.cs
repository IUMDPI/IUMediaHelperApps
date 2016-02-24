namespace Packager.Models.FileModels
{
    public class AudioPreservationIntermediateFile : AbstractPreservationIntermediateFile
    {
        private const string ExtensionValue = ".wav";

        public AudioPreservationIntermediateFile(AbstractFile original) : base(original, ExtensionValue)
        {
        }

        public override int Precedence => 1;
    }
}