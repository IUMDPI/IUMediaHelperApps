namespace Packager.Models.FileModels
{
    public class VideoPreservationIntermediateFile : AbstractPreservationIntermediateFile
    {
        public VideoPreservationIntermediateFile(AbstractFile original) : base(original)
        {
        }

        public override string Extension => ".mkv";
    }
}