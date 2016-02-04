namespace Packager.Models.FileModels
{
    public class ProductionFile : AbstractFile
    {
        public ProductionFile(AbstractFile original) : base(original)
        {
        }

        public override string FileUse => "prod";
        public override string FullFileUse => "Production Master";
        public override string Extension => ".wav";
    }
}