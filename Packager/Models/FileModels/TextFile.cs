using Common.Models;

namespace Packager.Models.FileModels
{
    public class TextFile : AbstractFile
    {
        private const string ExtensionValue = ".txt";

        public TextFile(AbstractFile original) : base(original, FileUsages.TextFile, ExtensionValue)
        {
            
        }
    }
}