using Common.Models;

namespace Packager.Models.FileModels
{
    public class FilesArchiveFile: AbstractFile
    {
        private const string ExtensionValue = ".zip";

        public FilesArchiveFile(AbstractFile original): base(original, ExtensionValue)
        {
        }

        public override int Precedence => 7;
    }
}