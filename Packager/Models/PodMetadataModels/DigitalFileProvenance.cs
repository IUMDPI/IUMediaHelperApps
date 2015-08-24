using System;
using System.Collections.Generic;
using System.Linq;
using Packager.Validators.Attributes;

namespace Packager.Models.PodMetadataModels
{
    public class DigitalFileProvenance
    {
        private const string AdDeviceType = "AD";
        private const string PlayerDeviceType = "Player";
        private const string ExtractionWorkstationDeviceType = "Extraction Workstation";

        [Required]
        public DateTime? DateDigitized { get; set; }

        public string Comment { get; set; }

        [Required]
        public string CreatedBy { get; set; }

        public List<Device> SignalChain { get; set; }

        [Required]
        public string SpeedUsed { get; set; }

        [Required]
        public string Filename { get; set; }
        
        [HasMembers]
        public IEnumerable<Device> PlayerDevices
        {
            get
            {
                return SignalChain?.
                    Where(e => e.DeviceType.Equals(PlayerDeviceType, StringComparison.InvariantCultureIgnoreCase)) ?? new List<Device>();
            }
        }

        [HasMembers]
        public IEnumerable<Device> AdDevices
        {
            get
            {
                return SignalChain?.
                    Where(e => e.DeviceType.Equals(AdDeviceType, StringComparison.InvariantCultureIgnoreCase)) ?? new List<Device>();
            }
        }
        
        [Required]
        public string ExtractionWorkstation {
            get
            {
                if (SignalChain == null)
                {
                    return "";
                }

                var entry = SignalChain
                    .FirstOrDefault(d => d.DeviceType.Equals(ExtractionWorkstationDeviceType, StringComparison.InvariantCultureIgnoreCase));
                return entry == null ? "" : $"{entry.Manufacturer} {entry.Model} {entry.SerialNumber}";
            } }
     
    }
}