using System;
using System.IO;
using Packager.Extensions;

namespace Packager.Models.FileModels
{
    public class UnknownFile : AbstractFile
    {
        public UnknownFile(string file) 
        {
            var parts = GetPathParts(file);

            ProjectCode = parts.FromIndex(0, string.Empty).ToUpperInvariant();
            BarCode = parts.FromIndex(1, string.Empty).ToLowerInvariant();
            SequenceIndicator = GetSequenceIndicator(parts.FromIndex(2, string.Empty));
            Extension = Path.GetExtension(file);
            FileUse = parts.FromIndex(3, string.Empty);
        }

        private static string[] GetPathParts(string path)
        {
            return Path.GetFileNameWithoutExtension(path)
                .ToDefaultIfEmpty()
                .ToLowerInvariant()
                .Split(new[] { '_' }, StringSplitOptions.RemoveEmptyEntries);
        }

        public override string Extension { get; }
        public override string FileUse { get; }
        public override string FullFileUse => "Raw object file";

        public override bool IsValid()
        {
            return false;
        }
    }
}