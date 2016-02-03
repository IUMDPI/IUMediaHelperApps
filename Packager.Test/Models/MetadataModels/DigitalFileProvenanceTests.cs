using System;
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

            private List<Device> MockDeviceList { get; set; }

            private List<Device> MockPlayerList { get; set; } 
            private List<Device> MockAdList { get; set; } 

            private Device MockExtractionWorkStation { get; set; }

            private AbstractDigitalFile Instance { get; set; }

            [SetUp]
            public virtual void BeforeEach()
            {
                Factory = Substitute.For<IImportableFactory>();
                SignalChainElement = new XElement("signal_chain");
                Element = new XElement("digital_file_provenance", SignalChainElement);

                Factory.ToLocalDateTimeValue(Element, "date_digitized").Returns(new DateTime(2016, 1, 1));
                Factory.ToStringValue(Element, "filename").Returns("filename value");
                Factory.ToStringValue(Element, "comment").Returns("comment value");
                Factory.ToStringValue(Element, "created_by").Returns("created by value");

                MockPlayerList = new List<Device>
                    {
                        new Device {DeviceType = "player", Model = "model", Manufacturer = "manufacturer", SerialNumber = "serial number"},
                        new Device {DeviceType = "player", Model = "model", Manufacturer = "manufacturer", SerialNumber = "serial number"},
                    };

                MockAdList = new List<Device>
                    {
                        new Device {DeviceType = "ad", Model = "model", Manufacturer = "manufacturer", SerialNumber = "serial number"},
                        new Device {DeviceType = "ad", Model = "model", Manufacturer = "manufacturer", SerialNumber = "serial number"},
                    };

                MockExtractionWorkStation = new Device
                {
                    DeviceType = "extraction workstation",
                    Model = "model",
                    Manufacturer = "manufacturer",
                    SerialNumber = "serial number"
                };
            }
            
            [Test]
            public void ItShouldUseCorrectPathToResolveDateDigitized()
            {
                Factory.Received().ToLocalDateTimeValue(Element, "date_digitized");
            }

            [Test]
            public void ItShouldSetDateDigitizedCorrectly()
            {
                Assert.That(Instance.DateDigitized.HasValue);
                Assert.That(Instance.DateDigitized.Value, Is.EqualTo(new DateTime(2016, 1, 1)));
            }

            [Test]
            public void ItShouldUseCorrectPathToResolveFilename()
            {
                Factory.Received().ToStringValue(Element, "filename");
            }

            [Test]
            public void ItShouldSetFilenameCorrectly()
            {
                Assert.That(Instance.Filename, Is.EqualTo("filename value"));
            }

            [Test]
            public void ItShouldUseCorrectPathToResolveComment()
            {
                Factory.Received().ToStringValue(Element, "comment");
            }

            [Test]
            public void ItShouldSetCommentCorrectly()
            {
                Assert.That(Instance.Comment, Is.EqualTo("comment value"));
            }

            [Test]
            public void ItShouldSetCreatedByCorrectly()
            {
                Assert.That(Instance.CreatedBy, Is.EqualTo("created by value"));
            }

            [Test]
            public void ItShouldSetAdDevicesCorrectly()
            {
                Assert.That(Instance.AdDevices, Is.EquivalentTo(MockAdList));
            }

            [Test]
            public void ItShouldSetPlayerDevicesCorrectly()
            {
                Assert.That(Instance.PlayerDevices, Is.EquivalentTo(MockPlayerList));
            }

            [Test]
            public void ItShouldSetExtractionWorkstationCorrectly()
            {
                Assert.That(Instance.ExtractionWorkstation, Is.EqualTo(MockExtractionWorkStation));
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
                private DigitalAudioFile DigitalAudioFile => Instance as DigitalAudioFile;

                [SetUp]
                public override void BeforeEach()
                {
                    base.BeforeEach();

                    MockDeviceList = new List<Device> {MockExtractionWorkStation};
                    MockDeviceList.AddRange(MockAdList);
                    MockDeviceList.AddRange(MockPlayerList);
                    
                    Factory.ToObjectList<Device>(SignalChainElement, "device").Returns(MockDeviceList);
                    Factory.ToStringValue(Element, "speed_used").Returns("speed used value");
                    Factory.ToStringValue(Element, "tape_fluxivity").Returns("reference fluxivity value");
                    Factory.ToStringValue(Element, "analog_output_voltage").Returns("analog output voltage value");
                    Factory.ToStringValue(Element, "peak").Returns("peak value");
                    Factory.ToStringValue(Element, "stylus_size").Returns("stylus size value");
                    Factory.ToStringValue(Element, "turnover").Returns("turnover value");
                    Factory.ToStringValue(Element, "volume_units").Returns("gain value");
                    Factory.ToStringValue(Element, "rolloff").Returns("rolloff value");

                    Instance = new DigitalAudioFile();
                    Instance.ImportFromXml(Element, Factory);
                }

                [Test]
                public void ItShouldUseCorrectPathToResolveSpeedUsed()
                {
                    Factory.Received().ToStringValue(Element, "speed_used");
                }

                [Test]
                public void ItShouldSetSpeedUsedCorrectly()
                {
                    Assert.That(DigitalAudioFile.SpeedUsed, Is.EqualTo("speed used value"));
                }

                [Test]
                public void ItShouldUseCorrectPathToResolveReferenceFluxivity()
                {
                    Factory.Received().ToStringValue(Element, "tape_fluxivity");
                }

                [Test]
                public void ItShouldSetReferenceFluxivityCorrectly()
                {
                    Assert.That(DigitalAudioFile.ReferenceFluxivity, Is.EqualTo("reference fluxivity value"));
                }

                [Test]
                public void ItShouldUseCorrectPathToResolveAnalogOutputVoltage()
                {
                    Factory.Received().ToStringValue(Element, "analog_output_voltage");
                }

                [Test]
                public void ItShouldSetAnalogOutoutVoltageCorrectly()
                {
                    Assert.That(DigitalAudioFile.AnalogOutputVoltage, Is.EqualTo("analog output voltage value"));
                }
                
                [Test]
                public void ItShouldUseCorrectPathToResolvePeak()
                {
                    Factory.Received().ToStringValue(Element, "peak");
                }

                [Test]
                public void ItShouldSetPeakCorrectly()
                {
                    Assert.That(DigitalAudioFile.Peak, Is.EqualTo("peak value"));
                }

                [Test]
                public void ItShouldUseCorrectPathToResolveStylusSize()
                {
                    Factory.Received().ToStringValue(Element, "stylus_size");
                }

                [Test]
                public void ItShouldSetStylusSizeCorrectly()
                {
                    Assert.That(DigitalAudioFile.StylusSize, Is.EqualTo("stylus size value"));
                }
                
                [Test]
                public void ItShouldUseCorrectPathToResolveTurnover()
                {
                    Factory.Received().ToStringValue(Element, "turnover");
                }


                [Test]
                public void ItShouldSetTurnoverCorrectly()
                {
                    Assert.That(DigitalAudioFile.Turnover, Is.EqualTo("turnover value"));
                }

                [Test]
                public void ItShouldUseCorrectPathToResolveGain()
                {
                    Factory.Received().ToStringValue(Element, "volume_units");
                }

                [Test]
                public void ItShouldSetGainCorrectly()
                {
                    Assert.That(DigitalAudioFile.Gain, Is.EqualTo("gain value"));
                }
                
                [Test]
                public void ItShouldUseCorrectPathToResolveRolloff()
                {
                    Factory.Received().ToStringValue(Element, "rolloff");
                }

                [Test]
                public void ItShouldSetRolloffCorrectly()
                {
                    Assert.That(DigitalAudioFile.Rolloff, Is.EqualTo("rolloff value"));
                }
            }

            public class WhenImportingDigitalVideoFiles : WhenImporting
            {
                private List<Device> MockTbcDevices { get; set; }
                private Device MockEncoder { get; set; }

                private DigitalVideoFile DigitalVideoFile => Instance as DigitalVideoFile;

                [SetUp]
                public override void BeforeEach()
                {
                    base.BeforeEach();

                    MockTbcDevices = new List<Device>
                    {
                        new Device {DeviceType = "tbc", Model = "model", Manufacturer = "manufacturer", SerialNumber = "serial number"},
                        new Device {DeviceType = "tbc", Model = "model", Manufacturer = "manufacturer", SerialNumber = "serial number"},
                    };

                    MockEncoder = new Device
                    {
                        DeviceType = "video capture",
                        Model = "model",
                        Manufacturer = "manufacturer",
                        SerialNumber = "serial number"
                    };

                    MockDeviceList = new List<Device> { MockExtractionWorkStation, MockEncoder };
                    MockDeviceList.AddRange(MockAdList);
                    MockDeviceList.AddRange(MockPlayerList);
                    MockDeviceList.AddRange(MockTbcDevices);

                    Factory.ToObjectList<Device>(SignalChainElement, "device").Returns(MockDeviceList);
                    
                    Instance = new DigitalVideoFile();
                    Instance.ImportFromXml(Element, Factory);
                }

                [Test]
                public void ItShouldSetTbcDevicesCorrectly()
                {
                    Assert.That(DigitalVideoFile.TBCDevices, Is.EquivalentTo(MockTbcDevices));
                }

                [Test]
                public void ItShouldSetEncoderCorrectly()
                {
                    Assert.That(DigitalVideoFile.Encoder, Is.EqualTo(MockEncoder));
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