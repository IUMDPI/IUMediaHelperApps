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
        private Device Instance { get; set; }

        [SetUp]
        public void BeforeEach()
        {
            // Fake the factory, as we're just interested in how it's being called
            Factory = Substitute.For<IImportableFactory>();

            // here we don't need to set values, as we're just testing that the factory
            // gets called correctly
            Element = new XElement("pod");

            Factory.ToStringValue(Element, "device_type").Returns("device type value");
            Factory.ToStringValue(Element, "serial_number").Returns("serial number value");
            Factory.ToStringValue(Element, "model").Returns("model value");
            Factory.ToStringValue(Element, "manufacturer").Returns("manufacturer value");

            Instance = new Device();
            Instance.ImportFromXml(Element, Factory);
        }

        [Test]
        public void ItShouldUseCorrectPathToResolveDeviceType()
        {
            Factory.Received().ToStringValue(Element, "device_type");
        }

        [Test]
        public void ItShouldSetDeviceTypeCorrectly()
        {
            Assert.That(Instance.DeviceType, Is.EqualTo("device type value"));
        }

        [Test]
        public void ItShouldUseCorrectPathToResolveSerialNumber()
        {
            Factory.Received().ToStringValue(Element, "serial_number");
        }

        [Test]
        public void ItShouldSetSerialNumberCorrectly()
        {
            Assert.That(Instance.SerialNumber, Is.EqualTo("serial number value"));
        }

        [Test]
        public void ItShouldUseCorrectPathToResolveModel()
        {
            Factory.Received().ToStringValue(Element, "model");
        }

        [Test]
        public void ItShouldSetModelCorrectly()
        {
            Assert.That(Instance.Model, Is.EqualTo("model value"));
        }

        [Test]
        public void ItShouldUseCorrectPathToResolveManufacturer()
        {
            Factory.Received().ToStringValue(Element, "manufacturer");
        }


        [Test]
        public void ItShouldSetManufacturerCorrectly()
        {
            Assert.That(Instance.Manufacturer, Is.EqualTo("manufacturer value"));
        }
    }
}