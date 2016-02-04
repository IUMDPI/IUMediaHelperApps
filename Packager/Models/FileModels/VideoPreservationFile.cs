namespace Packager.Models.FileModels
{
    public class VideoPreservationFile : AbstractPreservationFile
    {
        public VideoPreservationFile(AbstractFile original) : base(original)
        {
        }

        public override string Extension => ".mkv";
    }
}