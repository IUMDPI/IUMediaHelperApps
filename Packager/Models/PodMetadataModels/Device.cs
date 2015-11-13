using System;
using System.Xml.Linq;
using Packager.Deserializers;
using Packager.Extensions;
using Packager.Providers;

namespace Packager.Models.PodMetadataModels
{
    public class Device : IImportable
    {
        public string DeviceType { get; set; }
        public string SerialNumber { get; set; }
        public string Manufacturer { get; set; }
        public string Model { get; set; }

        public void ImportFromXml(XElement element, ILookupsProvider lookupsProvider)
        {
            DeviceType = element.ToStringValue("device_type");
            SerialNumber = element.ToStringValue("serial_number");
            Manufacturer = element.ToStringValue("manufacturer");
            Model = element.ToStringValue("model");
        }

        public void ImportFromXml(XDocument document)
        {
            throw new NotImplementedException();
        }
    }
}