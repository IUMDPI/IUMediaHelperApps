using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Packager.Attributes;

namespace Packager.Models
{
    public class PartsData
    {
        [ExcelField("DigitizingEntity", true)]
        public string DigitizingEntity { get; set; }

        public SideData Side1 { get; set; }
        public SideData Side2 { get; set; }
    }
}
