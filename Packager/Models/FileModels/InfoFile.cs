namespace Packager.Models.FileModels
{
    public class InfoFile : AbstractFile
    {
        private const string ExtensionValue = ".info";

        public InfoFile(AbstractFile original) : base(original, original.FileUsage, ExtensionValue)
        {
        }

        public override int Precedence => 10;
    }
}