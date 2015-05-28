using System;

namespace Packager.Models
{
    [Serializable]
    public class FileData
    {
        public string FileName { get; set; }
        public string Checksum { get; set; }
    }
}