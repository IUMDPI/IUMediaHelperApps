using System;
using System.Dynamic;
using System.Globalization;
using System.IO;
using System.Linq;
using Packager.Extensions;

namespace Packager.Models.FileModels
{
    public abstract class AbstractFile 
    {
        private string _projectCode;

        protected AbstractFile()
        {
            
        }
        
        protected AbstractFile(AbstractFile original)
        {
            BarCode = original.BarCode;
            SequenceIndicator = original.SequenceIndicator;
            ProjectCode = original.ProjectCode;
        }

        public string ProjectCode
        {
            get
            {
                return string.IsNullOrWhiteSpace(_projectCode)
                    ? string.Empty
                    : _projectCode.ToUpperInvariant();
            }
            set { _projectCode = value; }
        }

        public int SequenceIndicator { get; protected set; }
        public abstract string FileUse { get; }
        public string Checksum { get; set; }
        public abstract  string FullFileUse { get; }
        public string BarCode { get; set; }
        public abstract string Extension { get; }

        public string GetFolderName()
        {
            return $"{ProjectCode.ToUpperInvariant()}_{BarCode}";
        }

        public bool BelongsToProject(string projectCode)
        {
            return ProjectCode.Equals(projectCode, StringComparison.InvariantCultureIgnoreCase);
        }
        protected string ToFileNameWithoutExtension()
        {
            var parts = new[]
            {ProjectCode, BarCode, SequenceIndicator.ToString("D2", CultureInfo.InvariantCulture), FileUse};

            return string.Join("_", parts.Where(p => !string.IsNullOrWhiteSpace(p)));
        }

        public virtual string ToFileName()
        {
            return $"{ToFileNameWithoutExtension()}{Extension}";
        }

        protected static int GetSequenceIndicator(string value)
        {
            int result;
            return int.TryParse(value, out result)
                ? result
                : 0;
        }

        public virtual bool IsValid()
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

        public virtual bool IsSameAs(AbstractFile model)
        {
            if (!model.ProjectCode.Equals(ProjectCode, StringComparison.InvariantCultureIgnoreCase))
            {
                return false;
            }

            if (!model.BarCode.Equals(BarCode, StringComparison.InvariantCultureIgnoreCase))
            {
                return false;
            }

            if (model.SequenceIndicator != SequenceIndicator)
            {
                return false;
            }

            if (!model.FileUse.Equals(FileUse, StringComparison.InvariantCultureIgnoreCase))
            {
                return false;
            }

            if (!model.Extension.Equals(Extension, StringComparison.InvariantCultureIgnoreCase))
            {
                return false;
            }

            return true;
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