using Packager.Attributes;

namespace Packager.Models
{
    public class PreviewData
    {
        [ExcelField("Preview-Comments", false)]
        public string Comments { get; set; }
    }
}