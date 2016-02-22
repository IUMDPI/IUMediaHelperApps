namespace Packager.Models.FileModels
{
    public class AudioPreservationFile : AbstractPreservationFile
    {
        private const string ExtensionValue = ".wav";

        public AudioPreservationFile(AbstractFile original) : base(original, ExtensionValue)
        {
        }

        public override int Precedence => 0;
    }
}