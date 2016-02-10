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
            DeviceType = factory.ToStringValue(element,"device_type");
            SerialNumber = factory.ToStringValue(element,"serial_number");
            Manufacturer = factory.ToStringValue(element,"manufacturer");
            Model = factory.ToStringValue(element,"model");
        }
    }
}