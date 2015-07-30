using System;

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

        public override bool IsSameAs(string filename)
        {
            return filename.Equals(OriginalFileName, StringComparison.InvariantCultureIgnoreCase);
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