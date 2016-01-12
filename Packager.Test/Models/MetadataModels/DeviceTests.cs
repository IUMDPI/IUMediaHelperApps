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
        private Device Instance { get; set; }

        private XElement Element => new XElement("base", 
            new XElement("device_type") {Value = "device type value"},
            new XElement("serial_number") { Value = "serial number value" },
            new XElement("manufacturer") { Value = "manufacturer value" },
            new XElement("model") { Value = "model value" });

        [SetUp]
        public void BeforeEach()
        {
            Instance = new Device();
            Instance.ImportFromXml(Element, Substitute.For<IImportableFactory>());
        }

        [Test]
        public void AfterImportDeviceTypeShouldBeCorrect()
        {
            Assert.That(Instance.DeviceType, Is.EqualTo("device type value"));
        }

        [Test]
        public void AfterImportSerialNumberShouldBeCorrect()
        {
            Assert.That(Instance.SerialNumber, Is.EqualTo("serial number value"));
        }

        [Test]
        public void AfterImportManufacturerShouldBeCorrect()
        {
            Assert.That(Instance.Manufacturer, Is.EqualTo("manufacturer value"));
        }

        [Test]
        public void AfterImportModelShouldBeCorrect()
        {
            Assert.That(Instance.Model, Is.EqualTo("model value"));
        }
    }
}