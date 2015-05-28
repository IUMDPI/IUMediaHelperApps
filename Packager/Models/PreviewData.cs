using System;
using Packager.Attributes;

namespace Packager.Models
{
    [Serializable]
    public class PreviewData
    {
        [ExcelField("Preview-Comments", false)]
        public string Comments { get; set; }
    }
}