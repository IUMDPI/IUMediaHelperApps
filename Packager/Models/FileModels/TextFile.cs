namespace Packager.Models.FileModels
{
    public class TextFile : AbstractFile
    {
        private const string ExtensionValue = ".txt";

        public TextFile(AbstractFile original) : base(original, ExtensionValue)
        {
        }

        public override int Precedence => 9;
    }
}