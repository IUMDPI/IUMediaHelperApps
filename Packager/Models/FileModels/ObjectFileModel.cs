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
        private const string PreservationIntermediateFileUse = "presInt";
        private const string AccessFileUse = "access";
        private const string MezzanineFileUse = "mezz";
        private const string ProductionFileUse = "prod";
        private const string PreservationFileUseLongName = "Preservation Master";
        private const string ProductionFileUseLongName = "Production Master";
        private const string PreservationIntermediateFileUseLongName = "Preservation Master - Intermediate";
        private const string AccessFileUseLongName = "Access File Version";
        private const string MezzanineFileUseLongName = "Mezzanine File Version";

        public const string AudioAccessExtension = ".mp4";
        public const string VideoAccessExtension = ".mp4"; 
        public const string MezzanineExtension = ".mov"; 
        public const string ProductionExtension = ".wav"; 
        
        protected ObjectFileModel()
        {
        }

        public ObjectFileModel(string path)
            : base(path)
        {
            var parts = GetPathParts(path);

            SequenceIndicator = GetSequenceIndicator(parts.FromIndex(2, string.Empty));
            FileUse = parts.FromIndex(3, string.Empty)
                .ToLowerInvariant() // convert to lower case
                .Replace("presint", "presInt"); // now make sure presInt is cased correctly
        }

        public int SequenceIndicator { get; protected set; }
        public string FileUse { get; set; }
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

        protected string ToFileNameWithoutExtension()
        {
            var parts = new[]
            {ProjectCode, BarCode, SequenceIndicator.ToString("D2", CultureInfo.InvariantCulture), FileUse};

            return string.Join("_", parts.Where(p => !string.IsNullOrWhiteSpace(p)));
        }

        public override string ToFileName()
        {
            return $"{ToFileNameWithoutExtension()}{Extension}";
        }

        private static int GetSequenceIndicator(string value)
        {
            int result;
            return int.TryParse(value, out result)
                ? result
                : 0;
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

        private ObjectFileModel ToNewModel(string newFileUse, string newExtension)
        {
            var result = new ObjectFileModel
            {
                ProjectCode = ProjectCode,
                BarCode = BarCode,
                FileUse = newFileUse,
                Extension = newExtension,
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
            return FileUse.Equals(PreservationIntermediateFileUse, StringComparison.InvariantCultureIgnoreCase);
        }

        public bool IsMezzanineVersion()
        {
            return FileUse.Equals(MezzanineFileUse, StringComparison.InvariantCultureIgnoreCase);
        }

        private ObjectFileModel ToAccessFileModel(string extension)
        {
            return ToNewModel(AccessFileUse, extension);
        }

        public ObjectFileModel ToMezzanineFileModel()
        {
            return ToNewModel(MezzanineFileUse, MezzanineExtension);
        }

        public ObjectFileModel ToProductionFileModel()
        {
            return ToNewModel(ProductionFileUse, ProductionExtension);
        }

        public ObjectFileModel ToAudioAccessFileModel()
        {
            return ToAccessFileModel(AudioAccessExtension);
        }

        public ObjectFileModel ToVideoAccessFileModel()
        {
            return ToAccessFileModel(VideoAccessExtension);
        }

        public string ToFrameMd5Filename()
        {
            return $"{ToFileNameWithoutExtension()}.framemd5";
        }

        public string GetOriginalFolderName()
        {
            return Path.Combine(GetFolderName(), "Originals");
        }
    }
}