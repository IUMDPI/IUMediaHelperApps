using Common.Models;

namespace Packager.Models.FileModels
{
    public class CueFile : AbstractFile
    {
        private const string ExtensionValue = ".cue";

        public CueFile(AbstractFile original) : base(original, ExtensionValue)
        {
        }

        public override int Precedence => FileUsage == FileUsages.PreservationMaster ? 1 : 3;
    }
}