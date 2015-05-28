using Packager.Attributes;

namespace Packager.Models
{
    public class BakingData
    {
        [ExcelField("Baking-Date", false)]
        public string Date { get; set; }
    }
}