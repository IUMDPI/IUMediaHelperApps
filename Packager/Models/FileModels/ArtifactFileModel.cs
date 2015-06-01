using System;
using System.Linq;
using Packager.Extensions;

namespace Packager.Models.FileModels
{
    public class ArtifactFileModel : AbstractFileModel
    {
        private const string PreservationFileUse = "pres";
        private const string AccessFileUse = "access";
        private const string MezzanineFileUse = "mezz";
        private const string ProductionFileUse = "prod";

        private ArtifactFileModel()
        {
        }

        public ArtifactFileModel(string path)
            : base(path)
        {
            var parts = GetPathParts(path);

            SequenceIndicator = parts.FromIndex(2, string.Empty).ToLowerInvariant();
            FileUse = parts.FromIndex(3, string.Empty).ToLowerInvariant();
        }

        public string SequenceIndicator { get; set; }
        private string FileUse { get; set; }

        private ArtifactFileModel ToNewModel(string newFileUse, string newExension)
        {
            var result = new ArtifactFileModel
            {
                ProjectCode = ProjectCode,
                BarCode = BarCode,
                FileUse = newFileUse,
                Extension = newExension,
                SequenceIndicator = SequenceIndicator
            };

            result.OriginalFileName = result.ToFileName();
            return result;
        }

        public bool IsProductionVersion()
        {
            return FileUse.Equals(ProductionFileUse, StringComparison.InvariantCultureIgnoreCase);
        }

        public bool IsPreservationVersion()
        {
            return FileUse.Equals(PreservationFileUse, StringComparison.InvariantCultureIgnoreCase);
        }

        public bool IsAccessVersion()
        {
            return FileUse.Equals(AccessFileUse, StringComparison.InvariantCultureIgnoreCase);
        }

        public bool IsMezzanineVersion()
        {
            return FileUse.Equals(MezzanineFileUse, StringComparison.InvariantCultureIgnoreCase);
        }

        public ArtifactFileModel ToAccessFileModel(string extension)
        {
            return ToNewModel(AccessFileUse, extension);
        }

        public ArtifactFileModel ToMezzanineFileModel(string extension)
        {
            return ToNewModel(MezzanineFileUse, extension);
        }

        public ArtifactFileModel ToProductionFileModel(string extension)
        {
            return ToNewModel(ProductionFileUse, extension);
        }

        public override string ToFileName()
        {
            var parts = new[] {ProjectCode, BarCode, SequenceIndicator, FileUse};

            var fileName = string.Join("_", parts.Where(p => !string.IsNullOrWhiteSpace(p)));
            return string.Format("{0}{1}", fileName, Extension);
        }

        public override bool IsValid()
        {
            if (!base.IsValid())
            {
                return false;
            }

            if (String.IsNullOrWhiteSpace(SequenceIndicator))
            {
                return false;
            }

            if (String.IsNullOrWhiteSpace(FileUse))
            {
                return false;
            }

            return true;
        }
    }
}