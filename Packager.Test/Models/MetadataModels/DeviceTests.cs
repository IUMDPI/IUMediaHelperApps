using System.Xml.Linq;
using NSubstitute;
using NUnit.Framework;
using Packager.Factories;
using Packager.Models.PodMetadataModels;

namespace Packager.Test.Models.MetadataModels
{
    [TestFixture]
    public class DeviceTests
    {
        private IImportableFactory Factory { get; set; }
        private XElement Element { get; set; }

        [SetUp]
        public void BeforeEach()
        {
            // Fake the factory, as we're just interested in how it's being called
            Factory = Substitute.For<IImportableFactory>();

            // here we don't need to set values, as we're just testing that the factory
            // gets called correctly
           Element = new XElement("pod");

            var instance = new Device();
            instance.ImportFromXml(Element, Factory);
        }

        [Test]
        public void ItShouldUseCorrectPathToResolveDeviceType()
        {
            Factory.Received().ToStringValue(Element, "device_type");
        }

        [Test]
        public void ItShouldUseCorrectPathToResolveSerialNumber()
        {
            Factory.Received().ToStringValue(Element, "serial_number");
        }

        [Test]
        public void ItShouldUseCorrectPathToResolveModel()
        {
            Factory.Received().ToStringValue(Element, "model");
        }

        [Test]
        public void ItShouldUseCorrectPathToResolveManufacturer()
        {
            Factory.Received().ToStringValue(Element, "manufacturer");
        }

    }
}