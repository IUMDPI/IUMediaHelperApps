using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Packager.Deserializers;
using Packager.Extensions;
using Packager.Validators.Attributes;

namespace Packager.Models.PodMetadataModels.ConsolidatedModels
{
    public abstract class AbstractConsolidatedDigitalFile:IImportableFromPod
    {
        private const string AdDeviceType = "AD";
        private const string PlayerDeviceType = "Player";
        private const string ExtractionWorkstationDeviceType = "Extraction Workstation";

        protected AbstractConsolidatedDigitalFile(DigitalFileProvenance original)
        {
            DateDigitized = original.DateDigitized;
            //SignalChain = original.SignalChain;
            Filename = original.Filename;
            Comment = original.Comment;
            CreatedBy = original.CreatedBy;
        }

        protected AbstractConsolidatedDigitalFile()
        {
            
        }

        public virtual void ImportFromXml(XDocument document)
        {
            throw new NotImplementedException();
        }

        public virtual void ImportFromXml(XElement element)
        {
            DateDigitized = element.Element("date_digitized").GetValue((DateTime?) null);
            Filename = element.Element("filename").GetValue(string.Empty);
            Comment = element.Element("comment").GetValue(string.Empty);
            CreatedBy = element.Element("created_by").GetValue(string.Empty);
            SignalChain = ImportDevicesFromXml(element.Element("signal_chain"));
        }

        private List<Device> ImportDevicesFromXml(XElement element)
        {
            if (element == null)
            {
                return new List<Device>();
            }

            var result = new List<Device>();
            foreach (var deviceElement in element.Elements("device"))
            {
                var device = new Device();
                device.ImportFromXml(deviceElement);
                result.Add(device);
            }

            return result;
        }

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
        public Device ExtractionWorkstation
        {
            get
            {
                return SignalChain?.
                    FirstOrDefault(d => d.DeviceType.Equals(ExtractionWorkstationDeviceType, StringComparison.InvariantCultureIgnoreCase));
            }
        }

    }
}