namespace Packager.Models.FileModels
{
    public class VideoPreservationIntermediateFile : AbstractPreservationIntermediateFile
    {
        private const string ExtensionValue = ".mkv";

        public VideoPreservationIntermediateFile(AbstractFile original) : base(original, ExtensionValue)
        {
        }
    }
}