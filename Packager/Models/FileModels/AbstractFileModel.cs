using System;
using System.IO;
using Packager.Extensions;

namespace Packager.Models.FileModels
{
    public abstract class AbstractFileModel
    {
        private string _projectCode;

        protected AbstractFileModel()
        {
        }

        public abstract bool IsSameAs(string filename);

        protected AbstractFileModel(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
            {
                return;
            }

            OriginalFileName = Path.GetFileName(path);
            Extension = Path.GetExtension(path).ToDefaultIfEmpty();

            var parts = GetPathParts(path);

            ProjectCode = parts.FromIndex(0, string.Empty).ToUpperInvariant();
            BarCode = parts.FromIndex(1, string.Empty).ToLowerInvariant();
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

        public string OriginalFileName { get; set; }
        public string BarCode { get; set; }
        public string Extension { get; set; }

        protected static string[] GetPathParts(string path)
        {
            return Path.GetFileNameWithoutExtension(path)
                .ToDefaultIfEmpty()
                .ToLowerInvariant()
                .Split(new[] {'_'}, StringSplitOptions.RemoveEmptyEntries);
        }

        public abstract string ToFileName();

        public bool BelongsToProject(string projectCode)
        {
            return ProjectCode.Equals(projectCode, StringComparison.InvariantCultureIgnoreCase);
        }

        public bool HasExtension(string value)
        {
            return Extension.Equals(value, StringComparison.InvariantCultureIgnoreCase);
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

            return true;
        }

        
    }
}