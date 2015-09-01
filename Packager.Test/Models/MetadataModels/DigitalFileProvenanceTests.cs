using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using Packager.Models.PodMetadataModels;

namespace Packager.Test.Models.MetadataModels
{
    [TestFixture]
    public class DigitalFileProvenanceTests
    {
        private DigitalFileProvenance Source { get; set; }


        [SetUp]
        public void BeforeEach()
        {
            Source = new DigitalFileProvenance
            {
                SignalChain = new List<Device> {
                    new Device {DeviceType = "extraction workstation", Model = "ew model", Manufacturer = "ew manufacturer", SerialNumber = "ew serial number"},
                    new Device {DeviceType = "player", Model = "Player model", Manufacturer = "Player manufacturer", SerialNumber = "Player serial number"},
                    new Device {DeviceType = "ad", Model = "Ad model", Manufacturer = "Ad manufacturer", SerialNumber = "Ad serial number"}}
            };
        }

        [Test]
        public void AdDevicesShouldReturnCorrectResults()
        {
            var device = Source.AdDevices.FirstOrDefault();
            Assert.That(device, Is.Not.Null);
            Assert.That(device.SerialNumber, Is.EqualTo("Ad serial number"));
            Assert.That(device.Model, Is.EqualTo("Ad model"));
            Assert.That(device.Manufacturer, Is.EqualTo("Ad manufacturer"));
            
        }

        [Test]
        public void PlayerDevicesShouldReturnCorrectResults()
        {
            var device = Source.AdDevices.FirstOrDefault();
            Assert.That(device, Is.Not.Null);
            Assert.That(device.SerialNumber, Is.EqualTo("Ad serial number"));
            Assert.That(device.Model, Is.EqualTo("Ad model"));
            Assert.That(device.Manufacturer, Is.EqualTo("Ad manufacturer"));
        }

        [Test]
        public void ExtractionWorkstationShouldBeCorrect()
        {
            Assert.That(Source.ExtractionWorkstation.Model, Is.EqualTo("ew model"));
            Assert.That(Source.ExtractionWorkstation.Manufacturer, Is.EqualTo("ew manufacturer"));
            Assert.That(Source.ExtractionWorkstation.SerialNumber, Is.EqualTo("ew serial number"));
        }


    }
}
