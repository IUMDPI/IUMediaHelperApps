using System.Globalization;
using System.Linq;
using Packager.Extensions;

namespace Packager.Models.FileModels
{
    public abstract class AbstractObjectFileModel : AbstractFileModel
    {
        public int SequenceIndicator { get; set; }
        public string FileUse { get; set; }

        protected AbstractObjectFileModel()
        {
            
        }

        protected AbstractObjectFileModel(string path)
            : base(path)
        {
            var parts = GetPathParts(path);

            SequenceIndicator = GetSequenceIndicator(parts.FromIndex(2, string.Empty));
            FileUse = parts.FromIndex(3, string.Empty).ToLowerInvariant();
        }

        private static int GetSequenceIndicator(string value)
        {
            int result;
            return int.TryParse(value, out result)
                ? result
                : 0;
        }

        public override bool IsSameAs(string filename)
        {
            var model = new ObjectFileModel(filename);
            if (model.ProjectCode != ProjectCode)
            {
                return false;
            }

            if (model.BarCode != BarCode)
            {
                return false;
            }

            if (model.SequenceIndicator != SequenceIndicator)
            {
                return false;
            }

            if (model.FileUse != FileUse)
            {
                return false;
            }

            if (model.Extension != Extension)
            {
                return false;
            }

            return true;
        }

        protected string ToFileNameWithoutExtension()
        {
            var parts = new[] { ProjectCode, BarCode, SequenceIndicator.ToString("D2", CultureInfo.InvariantCulture), FileUse };

            return string.Join("_", parts.Where(p => !string.IsNullOrWhiteSpace(p)));
        }

        public override string ToFileName()
        {
            return $"{ToFileNameWithoutExtension()}{Extension}";
        }

        public override bool IsValid()
        {
            if (!base.IsValid())
            {
                return false;
            }

            if (SequenceIndicator < 1)
            {
                return false;
            }

            if (string.IsNullOrWhiteSpace(FileUse))
            {
                return false;
            }

            return true;
        }
    }
}