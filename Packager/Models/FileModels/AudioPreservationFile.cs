namespace Packager.Models.FileModels
{
    public class AudioPreservationFile : AbstractPreservationFile
    {
        public AudioPreservationFile(AbstractFile original) : base(original)
        {
        }

        public override string Extension => ".wav";
    }
}