namespace Packager.Models.FileModels
{
    public class AccessFile : AbstractFile
    {
        public AccessFile(AbstractFile original) : base(original)
        {
        }

        public override string FileUse => "access";
        public override string FullFileUse => "Access File Version";
        public override string Extension => ".mp4";
    }
}