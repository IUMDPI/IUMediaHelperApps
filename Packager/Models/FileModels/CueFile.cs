using Common.Models;

namespace Packager.Models.FileModels
{
    public class CueFile : AbstractFile
    {
        private const string ExtensionValue = ".cue";

        public CueFile(AbstractFile original) : base(original, FileUsages.CueFile, ExtensionValue)
        {

        }
    }
}