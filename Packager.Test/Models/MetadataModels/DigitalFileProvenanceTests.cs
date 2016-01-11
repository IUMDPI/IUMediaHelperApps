using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using NSubstitute;
using NUnit.Framework;
using Packager.Factories;
using Packager.Models.PodMetadataModels;
using Packager.Providers;

namespace Packager.Test.Models.MetadataModels
{
    [TestFixture]
    public abstract class DigitalFileProvenanceTests
    {
        private AbstractDigitalFile Source { get; set; }

        public class DigitalAudioFileTests : DigitalFileProvenanceTests
        {
            [SetUp]
            public void BeforeEach()
            {
                Source = new DigitalAudioFile
                {
                    SignalChain = new List<Device>
                    {
                        new Device {DeviceType = "extraction workstation", Model = "ew model", Manufacturer = "ew manufacturer", SerialNumber = "ew serial number"},
                        new Device {DeviceType = "player", Model = "Player model", Manufacturer = "Player manufacturer", SerialNumber = "Player serial number"},
                        new Device {DeviceType = "ad", Model = "Ad model", Manufacturer = "Ad manufacturer", SerialNumber = "Ad serial number"}
                    }
                };
            }

            public class WhenImporting 
            {
                private XElement Element { get; set; }
                private ILookupsProvider LookupsProvider { get; set; }

                private IImportableFactory Factory { get; set; }
                private DigitalAudioFile Result { get; set; }
                

                [SetUp]
                public void BeforeEach()
                {
                    LookupsProvider = Substitute.For<ILookupsProvider>();
                    Factory = Substitute.For<IImportableFactory>();

                    Element = new XElement("digital_file_provenance", 
                        new XElement("filename") {Value= "MDPI_40000001210717_01_pres.wav" },
                        new XElement("date_digitized") {Value= "2015-10-14T20:00:00-04:00" },
                        new XElement("comment") { Value = "comment" },
                        new XElement("created_by") { Value = "username" },
                        new XElement("speed_used") { Value="3.75 ips" },
                        new XElement("tape_fluxivity") { Value = "250" },
                        new XElement("volume_units") { Value = "+8" },
                        new XElement("analog_output_voltage") { Value = "+4" },
                        new XElement("peak") { Value = "-18" },
                        new XElement("stylus_size") {Value =  "2.0 ET"},
                        new XElement("turnover") {Value="turnover value"},
                        new XElement("rolloff") {Value="rolloff value"});

                    Result = new DigitalAudioFile();
                    Result.ImportFromXml(Element, Factory);
                }

                [Test]
                public void FilenameShouldBeCorrect()
                {
                    Assert.That(Result.Filename, Is.EqualTo("MDPI_40000001210717_01_pres.wav"));
                }

                [Test]
                public void DateDigitizedShouldBeCorrect()
                {
                    Assert.That(Result.DateDigitized.HasValue, Is.True);
                    Assert.That(Result.DateDigitized.Value, Is.EqualTo(DateTime.Parse("2015-10-14T20:00:00-04:00")));
                }

                [Test]
                public void CommentShouldBeCorrect()
                {
                    Assert.That(Result.Comment, Is.EqualTo("comment"));
                }

                [Test]
                public void CreatedByShouldBeCorrect()
                {
                    Assert.That(Result.CreatedBy, Is.EqualTo("username"));
                }

                [Test]
                public void SpeedUsedShouldBeCorrect()
                {
                    Assert.That(Result.SpeedUsed, Is.EqualTo("3.75 ips"));
                }

                [Test]
                public void ReferenceFluxivityShouldBeCorrect()
                {
                    Assert.That(Result.ReferenceFluxivity, Is.EqualTo("250 nWb/m"));
                }

                [Test]
                public void AnalogOutputVoltageShouldBeCorrect()
                {
                    Assert.That(Result.AnalogOutputVoltage, Is.EqualTo("+4 dBu"));
                }

                [Test]
                public void PeakShouldBeCorrect()
                {
                    Assert.That(Result.Peak, Is.EqualTo("-18 dBfs"));
                }

                [Test]
                public void StylusSizeShouldBeCorrect()
                {
                    Assert.That(Result.StylusSize, Is.EqualTo("2.0 ET"));
                }

                [Test]
                public void TurnoverShouldBeCorrect()
                {
                    Assert.That(Result.Turnover, Is.EqualTo("turnover value"));
                }

                [Test]
                public void GainShouldBeCorrect()
                {
                    Assert.That(Result.Gain, Is.EqualTo("+8 dB"));
                }

                [Test]
                public void RolloffShouldBeCorrect()
                {
                    Assert.That(Result.Rolloff, Is.EqualTo("rolloff value"));
                }


            }
        }

        public class DigitalVideoFileTests : DigitalFileProvenanceTests
        {
            [SetUp]
            public void BeforeEach()
            {
                Source = new DigitalVideoFile
                {
                    SignalChain = new List<Device>
                    {
                        new Device {DeviceType = "extraction workstation", Model = "ew model", Manufacturer = "ew manufacturer", SerialNumber = "ew serial number"},
                        new Device {DeviceType = "player", Model = "Player model", Manufacturer = "Player manufacturer", SerialNumber = "Player serial number"},
                        new Device {DeviceType = "ad", Model = "Ad model", Manufacturer = "Ad manufacturer", SerialNumber = "Ad serial number"},
                        new Device {DeviceType = "tbc", Model = "tbc model", Manufacturer = "tbc manufacturer", SerialNumber = "tbc serial number"},
                        new Device {DeviceType = "video capture", Model = "vc model", Manufacturer = "vc manufacturer", SerialNumber = "vc serial number"}
                    }
                };
            }

            private DigitalVideoFile DigitalFile => Source as DigitalVideoFile;

            [Test]
            public void EncoderShouldBeCorrect()
            {
                Assert.That(DigitalFile.Encoder.Model, Is.EqualTo("vc model"));
                Assert.That(DigitalFile.Encoder.Manufacturer, Is.EqualTo("vc manufacturer"));
                Assert.That(DigitalFile.Encoder.SerialNumber, Is.EqualTo("vc serial number"));
            }

            [Test]
            public void TbcDevicesShouldReturnCorrectResults()
            {
                var device = DigitalFile.TBCDevices.FirstOrDefault();
                Assert.That(device, Is.Not.Null);
                Assert.That(device.SerialNumber, Is.EqualTo("tbc serial number"));
                Assert.That(device.Model, Is.EqualTo("tbc model"));
                Assert.That(device.Manufacturer, Is.EqualTo("tbc manufacturer"));
            }
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
        public void ExtractionWorkstationShouldBeCorrect()
        {
            Assert.That(Source.ExtractionWorkstation.Model, Is.EqualTo("ew model"));
            Assert.That(Source.ExtractionWorkstation.Manufacturer, Is.EqualTo("ew manufacturer"));
            Assert.That(Source.ExtractionWorkstation.SerialNumber, Is.EqualTo("ew serial number"));
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
    }
}