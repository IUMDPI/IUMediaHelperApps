namespace Packager.Models.FileModels
{
    public class AudioPreservationIntermediateFile : AbstractPreservationIntermediateFile
    {
        public AudioPreservationIntermediateFile(AbstractFile original) : base(original)
        {
        }

        public override string Extension => ".wav";
    }
}