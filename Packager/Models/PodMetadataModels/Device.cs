using System;
using System.Xml.Linq;
using Packager.Extensions;
using Packager.Factories;

namespace Packager.Models.PodMetadataModels
{
    public class Device : IImportable
    {
        public string DeviceType { get; set; }
        public string SerialNumber { get; set; }
        public string Manufacturer { get; set; }
        public string Model { get; set; }

        public void ImportFromXml(XElement element, IImportableFactory factory)
        {
            DeviceType = element.ToStringValue("device_type");
            SerialNumber = element.ToStringValue("serial_number");
            Manufacturer = element.ToStringValue("manufacturer");
            Model = element.ToStringValue("model");
        }
    }
}