using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using NSubstitute;
using NUnit.Framework;
using Packager.Factories;
using Packager.Models.PodMetadataModels;

namespace Packager.Test.Models.MetadataModels.DigitalFileTests
{
    [TestFixture]
    public abstract class DigitalFileProvenanceTests
    {
        public abstract class WhenImporting : DigitalFileProvenanceTests
        {
            private IImportableFactory Factory { get; set; }
            private XElement Element { get; set; }
            private XElement SignalChainElement { get; set; }

            [SetUp]
            public virtual void BeforeEach()
            {
                Factory = Substitute.For<IImportableFactory>();
                SignalChainElement = new XElement("signal_chain");
                Element = new XElement("digital_file_provenance", SignalChainElement);
            }

            [Test]
            public void ItShouldUseCorrectPathToResolveDateDigitized()
            {
                Factory.Received().ToUtcDateTimeValue(Element, "date_digitized");
            }

            [Test]
            public void ItShouldUseCorrectPathToResolveFilename()
            {
                Factory.Received().ToStringValue(Element, "filename");
            }

            [Test]
            public void ItShouldUseCorrectPathToResolveComment()
            {
                Factory.Received().ToStringValue(Element, "comment");
            }

            [Test]
            public void ItShouldUseCorrectPathToResolveCreatedBy()
            {
                Factory.Received().ToStringValue(Element, "created_by");
            }

            [Test]
            public void ItShouldCallFactoryCorrectlyToImportDevices()
            {
                Factory.Received().ToObjectList<Device>(SignalChainElement, "device");
            }

            public class WhenImportingDigitalAudioFiles : WhenImporting
            {
                [SetUp]
                public override void BeforeEach()
                {
                    base.BeforeEach();

                    var instance = new DigitalAudioFile();
                    instance.ImportFromXml(Element, Factory);
                }

                [Test]
                public void ItShouldUseCorrectPathToResolveSpeedUsed()
                {
                    Factory.Received().ToStringValue(Element, "speed_used");
                }

                [Test]
                public void ItShouldUseCorrectPathToResolveTapeFluxivity()
                {
                    Factory.Received().ToStringValue(Element, "tape_fluxivity", " nWb/m");
                }

                [Test]
                public void ItShouldUseCorrectPathToResolveAnalogOutputVoltage()
                {
                    Factory.Received().ToStringValue(Element, "analog_output_voltage", " dBu");
                }

                [Test]
                public void ItShouldUseCorrectPathToResolvePeak()
                {
                    Factory.Received().ToStringValue(Element, "peak", " dBfs");
                }

                [Test]
                public void ItShouldUseCorrectPathToResolveStylusSize()
                {
                    Factory.Received().ToStringValue(Element, "stylus_size");
                }

                [Test]
                public void ItShouldUseCorrectPathToResolveTurnover()
                {
                    Factory.Received().ToStringValue(Element, "turnover");
                }

                [Test]
                public void ItShouldUseCorrectPathToResolveGain()
                {
                    Factory.Received().ToStringValue(Element, "volume_units", " dB");
                }

                [Test]
                public void ItShouldUseCorrectPathToResolveRolloff()
                {
                    Factory.Received().ToStringValue(Element, "rolloff");
                }
            }

            public class WhenImportingDigitalVideoFiles : WhenImporting
            {
                [SetUp]
                public override void BeforeEach()
                {
                    base.BeforeEach();
                    var instance = new DigitalVideoFile();
                    instance.ImportFromXml(Element, Factory);
                }

                // here DigitalVideoFile has no importable properties outside
                // of those defined in AbstractDigitalFile
                // but we still want to run the tests in the abstract class
                // after we call ImportFromXml on a DigitalVideoFile
            }
        }

        public abstract class WhenGettingSignalChainMembers : DigitalFileProvenanceTests
        {
            private AbstractDigitalFile Source { get; set; }

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

            public class WhenGettingDigitalAudioFileSignalChainMembers : WhenGettingSignalChainMembers
            {
                [SetUp]
                public void BeforeEach()
                {
                    Source = new DigitalAudioFile
                    {
                        SignalChain = new List<Device>
                        {
                            new Device
                            {
                                DeviceType = "extraction workstation",
                                Model = "ew model",
                                Manufacturer = "ew manufacturer",
                                SerialNumber = "ew serial number"
                            },
                            new Device
                            {
                                DeviceType = "player",
                                Model = "Player model",
                                Manufacturer = "Player manufacturer",
                                SerialNumber = "Player serial number"
                            },
                            new Device
                            {
                                DeviceType = "ad",
                                Model = "Ad model",
                                Manufacturer = "Ad manufacturer",
                                SerialNumber = "Ad serial number"
                            }
                        }
                    };
                }
            }

            public class WhenGettingDigitalVideoFileSignalChainMembers : WhenGettingSignalChainMembers
            {
                [SetUp]
                public void BeforeEach()
                {
                    Source = new DigitalVideoFile
                    {
                        SignalChain = new List<Device> {
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
        }
    }
}