using Common.Models;

namespace Packager.Models.FileModels
{
    public class MezzanineFile : AbstractFile
    {
        private const string ExtensionValue = ".mov";

        public MezzanineFile(AbstractFile original) :
            base(original, FileUsages.MezzanineFile, ExtensionValue)
        {
        }
    }
}