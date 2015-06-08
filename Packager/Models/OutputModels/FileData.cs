using System;

namespace Packager.Models.OutputModels
{
    [Serializable]
    public class FileData
    {
        public string FileName { get; set; }
        public string Checksum { get; set; }
    }
}