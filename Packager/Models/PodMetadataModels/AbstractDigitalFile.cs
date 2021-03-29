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
        private const string AdDeviceType1 = "AD";
        private const string AdDeviceType2 = "A/D";
        private const string PlayerDeviceType = "Player";
        private const string PreampDeviceType1 = "Preamp";
        private const string PreampDeviceType2 = "Pre amp";
        private const string ExtractionWorkstationDeviceType = "Extraction Workstation";

        [Required]
        public DateTime? DateDigitized { get; set; }
        public string Comment { get; set; }
        [Required]
        public string CreatedBy { get; set; }
        public List<Device> SignalChain { get; set; }
        [Required]
        public string Filename { get; set; }
        [Required]
        public string BextFile { get; set; }

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
                    Where(e => e.DeviceType.Equals(AdDeviceType1, StringComparison.InvariantCultureIgnoreCase) || 
                          e.DeviceType.Equals(AdDeviceType2, StringComparison.InvariantCultureIgnoreCase))  ??
                       new List<Device>();
            }
        }

        public IEnumerable<Device> PreampDevices
        {
            get
            {
                return SignalChain?.
                    Where(e => e.DeviceType.Equals(PreampDeviceType1, StringComparison.InvariantCultureIgnoreCase) ||
                        e.DeviceType.Equals(PreampDeviceType2, StringComparison.InvariantCultureIgnoreCase)) ??
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
            BextFile = factory.ToStringValue(element, "digital_file_bext");
        }
    }
}