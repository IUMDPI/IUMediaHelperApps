using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Packager.Models;

namespace Packager.Utilities
{
    class StandInBextDataProvider : IBextDataProvider
    {
        public BextData GetMetadata(string barcode)
        {
            return new BextData
            {
                Description = "Indiana University, Bloomington. William and Gayle Cook Music Library. TP-S .A1828 81-4-17 v. 1. File use:",
                IARL = "Indiana University, Bloomington. William and Gayle Cook Music Library. TP-S .A1828 81-4-17 v. 1. File use:",
                ICMT = "Indiana University, Bloomington. William and Gayle Cook Music Library."
            };
        }
    }
}
