using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Packager.Deserializers;
using Packager.Extensions;
using Packager.Factories;
using Packager.Validators.Attributes;

namespace Packager.Models.PodMetadataModels
{
    public abstract class AbstractDigitalFile : IImportable
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
        public string Filename { get; set; }

        [HasMembers]
        public IEnumerable<Device> PlayerDevices
        {
            get
            {
                return SignalChain?.
                    Where(e => e.DeviceType.Equals(PlayerDeviceType, StringComparison.InvariantCultureIgnoreCase)) ??
                       new List<Device>();
            }
        }

        [HasMembers]
        public IEnumerable<Device> AdDevices
        {
            get
            {
                return SignalChain?.
                    Where(e => e.DeviceType.Equals(AdDeviceType, StringComparison.InvariantCultureIgnoreCase)) ??
                       new List<Device>();
            }
        }

        [Required]
        public Device ExtractionWorkstation
        {
            get
            {
                return SignalChain?.
                    FirstOrDefault(
                        d =>
                            d.DeviceType.Equals(ExtractionWorkstationDeviceType,
                                StringComparison.InvariantCultureIgnoreCase));
            }
        }

        public virtual void ImportFromXml(XElement element, IImportableFactory factory)
        {
            DateDigitized = element.ToDateTimeValue("date_digitized");
            Filename = element.ToStringValue("filename");
            Comment = element.ToStringValue("comment");
            CreatedBy = element.ToStringValue("created_by");
            SignalChain = ImportDevices(element.Element("signal_chain"), factory);
        }

        private static List<Device> ImportDevices(XContainer element, IImportableFactory factory)
        {
            if (element == null)
            {
                return new List<Device>();
            }

            var result = new List<Device>();
            foreach (var deviceElement in element.Elements("device"))
            {
                var device = new Device();
                device.ImportFromXml(deviceElement, factory);
                result.Add(device);
            }

            return result;
        }
    }
}