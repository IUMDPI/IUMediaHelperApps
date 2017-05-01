namespace Packager.Models.FileModels
{
    public class QualityControlFile : AbstractFile
    {
        private const string ExtensionValue = ".qctools.xml.gz";
        private const string IntermediateExtensionValue = ".qctools.xml";

        public string IntermediateFileName { get; }

        public QualityControlFile(AbstractFile original) : 
            base(original, original.FileUsage, ExtensionValue)
        {
            Filename = $"{original.Filename}{ExtensionValue}";
            IntermediateFileName = $"{original.Filename}{IntermediateExtensionValue}";
        }

        public override int Precedence => 7;
    }
}