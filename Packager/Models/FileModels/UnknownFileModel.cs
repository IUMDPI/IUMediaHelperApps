namespace Packager.Models.FileModels
{
    public class UnknownFileModel : AbstractFileModel
    {
        public UnknownFileModel()
        {
        }

        public UnknownFileModel(string path) : base(path)
        {
        }

        public override string ToFileName()
        {
            return OriginalFileName;
        }

        public override bool IsValid()
        {
            return false;
        }
    }
}