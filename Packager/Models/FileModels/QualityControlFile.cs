namespace Packager.Models.FileModels
{
    public class QualityControlFile : AbstractFile
    {
        public QualityControlFile(AbstractFile original) : base(original)
        {
            FileUse = original.FileUse;
            FullFileUse = original.FullFileUse;
        }

        public override string FileUse { get; }
        public override string FullFileUse { get; }

        public override string Extension => ".qctools.xml.gz";

        public string ToIntermediateFileName()
        {
            return $"{ToFileNameWithoutExtension()}.qctools.xml";
        }
    }
}