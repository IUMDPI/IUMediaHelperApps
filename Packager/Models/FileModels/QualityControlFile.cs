namespace Packager.Models.FileModels
{
    public class QualityControlFile : AbstractFile
    {
        private readonly string _originalExtension;

        public QualityControlFile(AbstractFile original) : base(original)
        {
            FileUse = original.FileUse;
            FullFileUse = original.FullFileUse;
            _originalExtension = original.Extension;
        }

        public override string FileUse { get; }
        public override string FullFileUse { get; }

        public override string Extension => ".qctools.xml.gz";

        public override string ToFileName()
        {
            return $"{ToFileNameWithoutExtension()}{_originalExtension}{Extension}";
        }

        public string ToIntermediateFileName()
        {
            return $"{ToFileNameWithoutExtension()}{_originalExtension}.qctools.xml";
        }
    }
}