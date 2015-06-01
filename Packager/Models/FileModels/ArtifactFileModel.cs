using System;
using System.IO;
using System.Linq;
using Packager.Extensions;

namespace Packager.Models.FileModels
{
    public class ArtifactFileModel : AbstractFileModel
    {
        public const string PreservationFileUse = "pres";
        public const string AccessFileUse = "access";
        public const string MezzanineFileUse = "mezz";
        public const string ProductionFileUse = "prod";

        public string SequenceIndicator { get; set; }
        public string FileUse { get; set; }

        public ArtifactFileModel()
        {
        }

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

        public ArtifactFileModel(string path)
            : base(path)
        {
            var parts = Path.GetFileNameWithoutExtension(path)
                .ToDefaultIfEmpty()
                .ToLowerInvariant()
                .Split(new[] { '_' }, StringSplitOptions.RemoveEmptyEntries);

            SequenceIndicator = parts.FromIndex(2, string.Empty).ToLowerInvariant();
            FileUse = parts.FromIndex(3, string.Empty).ToLowerInvariant();
        }


        /*  public bool IsValidForGrouping()
          {
              if (string.IsNullOrWhiteSpace(ProjectCode))
              {
                  return false;
              }

              if (string.IsNullOrWhiteSpace(BarCode))
              {
                  return false;
              }

              if (string.IsNullOrWhiteSpace(Extension))
              {
                  return false;
              }

              return true;
          }

          public bool IsValidForProcessing()
          {
              if (!IsValidForGrouping())
              {
                  return false;
              }

              if (string.IsNullOrWhiteSpace(SequenceIndicator))
              {
                  return false;
              }

              if (string.IsNullOrWhiteSpace(FileUse))
              {
                  return false;
              }

              return true;
          }*/


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
            var parts = new[] { ProjectCode, BarCode, SequenceIndicator, FileUse };

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