using System.Collections.Generic;
using Packager.Validators.Attributes;

namespace Packager.Models.PodMetadataModels
{
    public class DigitalFileProvenance
    {
        [Required]
        public string DateDigitized { get; set; }

        public string Comment { get; set; }

        [Required]
        public string CreatedBy { get; set; }

        public List<Device> SignalChain { get; set; }

        [Required]
        public string SpeedUsed { get; set; }

        [Required]
        public string Filename { get; set; }



        //-----------------------------------------------------
        // deprecated 

        //[Required]
        public string CreatedAt { get; set; }

        //[Required]
        public string UpdatedAt { get; set; }
        
        //[Required]
        public string PlayerSerialNumber { get; set; }

        //[Required]
        public string PlayerManufacturer { get; set; }

        //[Required]
        public string PlayerModel { get; set; }

        //[Required]
        public string AdSerialNumber { get; set; }

        //[Required]
        public string AdManufacturer { get; set; }

        //[Required]
        public string AdModel { get; set; }

        //[Required]
        public string ExtractionWorkstation { get; set; }

        public string PreAmpSerialNumber { get; set; }
        public string PreAmp { get; set; }
    }
}