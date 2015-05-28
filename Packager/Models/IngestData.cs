using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Packager.Models
{
    [Serializable]
    public class IngestData
    {
        public string Date { get; set; }
        public string Comments { get; set; }
        public string CreatedBy { get; set; }
        public string PlayerSerialNumber { get; set; }
        public string PlayerManufacturer { get; set; }
        public string AdSerialNumber { get; set; }
        public string AdManufacturer { get; set; }
        public string AdModel { get; set; }
        public string ExtractionWorkstation { get; set; }
        public string SpeedUsed { get; set; }
    }
}
