using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Packager.Attributes;

namespace Packager.Models
{
    [Serializable]
    public class CleaningData
    {
        [ExcelField("Cleaning-Date", false)]
        public string Date { get; set; }
    }
}
