using System;
using System.Collections.Generic;
using System.Linq;
using Packager.Validators.Attributes;

namespace Packager.Models.PodMetadataModels
{
    public class DigitalVideoFile : AbstractDigitalFile
    {
        private const string VideoCaptureDeviceType = "video capture";
        private const string TBCDeviceType = "tbc";

        [Required]
        public Device Encoder
        {
            get
            {
                return SignalChain?.
                    FirstOrDefault(d => d.DeviceType.Equals(VideoCaptureDeviceType, StringComparison.InvariantCultureIgnoreCase));
            }
        }

        [HasMembers]
        public IEnumerable<Device> TBCDevices
        {
            get
            {
                return SignalChain?.
                    Where(e => e.DeviceType.Equals(TBCDeviceType, StringComparison.InvariantCultureIgnoreCase)) ?? new List<Device>();
            }
        }

    }
}