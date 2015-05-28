using System;
using Packager.Attributes;

namespace Packager.Models
{
    [Serializable]
    public class BakingData
    {
        [ExcelField("Baking-Date", false)]
        public string Date { get; set; }
    }
}