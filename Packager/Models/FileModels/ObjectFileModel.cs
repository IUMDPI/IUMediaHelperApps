using System;
using System.IO;

namespace Packager.Models.FileModels
{
    public class ObjectFileModel : AbstractObjectFileModel
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
        
        public const string AudioAccessExtension = ".mp4";
        public const string VideoAccessExtension = ".mp4"; //todo: verify
        public const string MezzanineExtension = ".mov"; //todo: verify
        public const string ProductionExtension = ".wav"; //todo:verify
        
        
        private ObjectFileModel()
        {
        }

        public ObjectFileModel(string path)
            : base(path)
        {
        }
        
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
            return FileUse.Equals(PreservationItermediateFileUse, StringComparison.InvariantCultureIgnoreCase);
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