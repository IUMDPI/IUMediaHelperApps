using Common.Extensions;
using Common.Models;

namespace Packager.Models.FileModels
{
    public class TextFile : AbstractFile
    {
        private const string ExtensionValue = ".txt";

        public TextFile(AbstractFile original) : base(original, ExtensionValue)
        {
            FileUsage= FileUsages.TextFile;
            SequenceIndicator = 1;
        }

        protected override string NormalizeFilename()
        {
            var parts = new[]
            {
                ProjectCode,
                BarCode
            };

            return string.Join("_", parts) + Extension;
        }

        public override bool IsImportable()
        {
            if (ProjectCode.IsNotSet())
            {
                return false;
            }

            if (BarCode.IsNotSet())
            {
                return false;
            }

            if (Extension.IsNotSet())
            {
                return false;
            }
                      
            if (!FileUsages.IsImportable(FileUsage))
            {
                return false;
            }

            return true;
        }

        public override int Precedence => 9;
    }
}