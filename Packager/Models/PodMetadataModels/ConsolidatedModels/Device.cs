using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Packager.Deserializers;
using Packager.Extensions;

namespace Packager.Models.PodMetadataModels.ConsolidatedModels
{
    public class Device:IImportableFromPod
    {
        public string DeviceType { get; set; }
        public string SerialNumber { get; set; }
        public string Manufacturer { get; set; }
        public string Model { get; set; }

        public void ImportFromXml(XDocument document)
        {
            throw new NotImplementedException();
        }

        public void ImportFromXml(XElement element)
        {
            DeviceType = element.Element("device_type").GetValue(string.Empty);
            SerialNumber = element.Element("serial_number").GetValue(string.Empty);
            Manufacturer = element.Element("manufacturer").GetValue(string.Empty);
            Model = element.Element("model").GetValue(string.Empty);
        }
    }
}
