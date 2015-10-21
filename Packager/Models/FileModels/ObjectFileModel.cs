using System;
using System.Globalization;
using System.IO;
using System.Linq;
using Packager.Extensions;

namespace Packager.Models.FileModels
{
    public class ObjectFileModel : AbstractFileModel
    {
        private const string PreservationFileUse = "pres";
        private const string PreservationItermediateFileUse = "pres-int";
        private const string AccessFileUse = "access";
        private const string MezzanineFileUse = "mezz";
        private const string ProductionFileUse = "prod";
        private const string PreservationFileUseLongName = "Preservation Master";
        private const string ProductionFileUseLongName = "Production Master";
        private const string PreservationIntermediateFileUseLongName = "Preservation Master - Intermediate";
        private const string AccessFileUseLongName = "Access File Version";
        private const string MezzanineFileUseLongName = "Mezzanine File Version";

        private ObjectFileModel()
        {
        }

        public ObjectFileModel(string path)
            : base(path)
        {
            var parts = GetPathParts(path);

            SequenceIndicator = GetSequenceIndicator(parts.FromIndex(2, string.Empty));
            FileUse = parts.FromIndex(3, string.Empty).ToLowerInvariant();
        }

        public int SequenceIndicator { get; set; }
        private string FileUse { get; set; }

        public string Checksum { get; set; }

        public string FullFileUse
        {
            get
            {
                if (IsPreservationIntermediateVersion())
                {
                    return PreservationIntermediateFileUseLongName;
                }

                if (IsPreservationVersion())
                {
                    return PreservationFileUseLongName;
                }

                if (IsProductionVersion())
                {
                    return ProductionFileUseLongName;
                }

                if (IsAccessVersion())
                {
                    return AccessFileUseLongName;
                }

                if (IsMezzanineVersion())
                {
                    return MezzanineFileUseLongName;
                }

                return "Unknown";
            }
        }

        private static int GetSequenceIndicator(string value)
        {
            int result;
            return int.TryParse(value, out result) 
                ? result
                : 0;
        }

        private ObjectFileModel ToNewModel(string newFileUse, string newExension)
        {
            var result = new ObjectFileModel
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

        public bool IsPreservationIntermediateVersion()
        {
            return FileUse.Equals(PreservationItermediateFileUse, StringComparison.InvariantCultureIgnoreCase);
        }

        public bool IsMezzanineVersion()
        {
            return FileUse.Equals(MezzanineFileUse, StringComparison.InvariantCultureIgnoreCase);
        }

        public ObjectFileModel ToAccessFileModel(string extension)
        {
            return ToNewModel(AccessFileUse, extension);
        }

        public ObjectFileModel ToMezzanineFileModel(string extension)
        {
            return ToNewModel(MezzanineFileUse, extension);
        }

        public ObjectFileModel ToProductionFileModel(string extension)
        {
            return ToNewModel(ProductionFileUse, extension);
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

        public override string ToFileName()
        {
            return $"{ToFileNameWithoutExtension()}{Extension}";
        }

        public string ToFrameMd5Filename()
        {
            
            return $"{ToFileNameWithoutExtension()}.framemd5";
        }

        private string ToFileNameWithoutExtension()
        {
            var parts = new[] { ProjectCode, BarCode, SequenceIndicator.ToString("D2", CultureInfo.InvariantCulture), FileUse };

            return string.Join("_", parts.Where(p => !string.IsNullOrWhiteSpace(p)));
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

            if (String.IsNullOrWhiteSpace(FileUse))
            {
                return false;
            }

            return true;
        }

        public string GetOriginalFolderName()
        {
            return Path.Combine(GetFolderName(), "Originals");
        }
    }
}