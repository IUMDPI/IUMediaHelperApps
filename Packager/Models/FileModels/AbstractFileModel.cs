using System;
using System.IO;
using Packager.Extensions;

namespace Packager.Models.FileModels
{
    public abstract class AbstractFileModel
    {
        protected AbstractFileModel()
        {
        }

        protected AbstractFileModel(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
            {
                return;
            }

            OriginalFileName = Path.GetFileName(path);
            Extension = Path.GetExtension(path).ToDefaultIfEmpty();

            var parts = Path.GetFileNameWithoutExtension(path)
                .ToDefaultIfEmpty()
                .ToLowerInvariant()
                .Split(new[] { '_' }, StringSplitOptions.RemoveEmptyEntries);

            ProjectCode = parts.FromIndex(0, string.Empty).ToLowerInvariant();
            BarCode = parts.FromIndex(1, string.Empty).ToLowerInvariant();
        }

        public string OriginalFileName { get; set; }
        public string ProjectCode { get; set; }
        public string BarCode { get; set; }
        public string Extension { get; set; }

        public abstract string ToFileName();
        
        public bool BelongsToProject(string projectCode)
        {
            return ProjectCode.Equals(projectCode, StringComparison.InvariantCultureIgnoreCase);
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