namespace Packager.Models.FileModels
{
    public class MezzanineFile : AbstractFile
    {
        public MezzanineFile(AbstractFile original) : base(original)
        {
        }

        public override string FileUse => "mezz";
        public override string FullFileUse => "Mezzanine File Version";
        public override string Extension => ".mov";
    }
}