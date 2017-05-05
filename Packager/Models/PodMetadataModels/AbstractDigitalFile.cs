using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Packager.Factories;
using Packager.Validators.Attributes;

namespace Packager.Models.PodMetadataModels
{
    public abstract class AbstractDigitalFile : IImportable
    {
        private const string AdDeviceType = "AD";
        private const string PlayerDeviceType = "Player";
        private const string PreampDeviceType = "Preamp";
        private const string ExtractionWorkstationDeviceType = "Extraction Workstation";

        [Required]
        public DateTime? DateDigitized { get; set; }
        public string Comment { get; set; }
        [Required]
        public string CreatedBy { get; set; }
        public List<Device> SignalChain { get; set; }
        [Required]
        public string Filename { get; set; }
        public string Bext { get; set; }

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

        public IEnumerable<Device> PreampDevices
        {
            get
            {
                return SignalChain?.
                    Where(e => e.DeviceType.Equals(PreampDeviceType, StringComparison.InvariantCultureIgnoreCase)) ??
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

        // xpath queries here are against each digital_file_provenance node in
        // digital_files
        public virtual void ImportFromXml(XElement element, IImportableFactory factory)
        {
            DateDigitized = factory.ToDateTimeValue(element, "date_digitized");
            Filename = factory.ToStringValue(element, "filename");
            Comment = factory.ToStringValue(element, "comment");
            CreatedBy = factory.ToStringValue(element, "created_by");
            SignalChain = factory.ToObjectList<Device>(element.Element("signal_chain"), "device");
            Bext = factory.ToStringValue(element, "digital_file_bext");
        }
    }
}