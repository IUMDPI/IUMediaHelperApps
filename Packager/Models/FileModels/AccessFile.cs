using Common.Models;

namespace Packager.Models.FileModels
{
    public class AccessFile : AbstractFile
    {
        private const string ExtensionValue = ".mp4";

        public AccessFile(AbstractFile original) : 
            base(original, FileUsages.AccessFile, ExtensionValue)
        {
        }
    }
}