using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Packager.Exceptions;
using Packager.Factories;
using Packager.Models.FileModels;
using Packager.Models.OutputModels.Ingest;
using Packager.Models.PodMetadataModels;
using Device = Packager.Models.PodMetadataModels.Device;

namespace Packager.Test.Factories
{
    [TestFixture]
    public class VideoIngestFactoryTests
    {
        private const string GoodFileName = "MDPI_4890764553278906_01_pres.mkv";
        private const string MissingFileName = "MDPI_111111111111_01_pres.mkv";

        private VideoPodMetadata PodMetadata { get; set; }
        private AbstractFileModel FileModel { get; set; }
        private DigitalVideoFile Provenance { get; set; }
        private VideoIngest Result { get; set; }

        private static DigitalVideoFile GenerateFileProvenance(string fileName)
        {
            return new DigitalVideoFile
            {
                Comment = "Comment",
                CreatedBy = "Created by",
                DateDigitized = new DateTime(2015, 8, 1, 1, 2, 3),
                Filename = fileName,
                SignalChain = GetSignalChain()
            };
        }

        private static List<Device> GetSignalChain()
        {
            var result = new List<Device>
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
                },
                new Device
                {
                    DeviceType = "tbc",
                    Model = "tbc model",
                    Manufacturer = "tbc manufacturer",
                    SerialNumber = "tbc serial number"
                },
                new Device
                {
                    DeviceType = "video capture",
                    SerialNumber = "vc serial number",
                    Manufacturer = "vc manufacturer",
                    Model = "vc model"
                }
            };

            return result;
        }

        public class WhenDateDigitizedIsNotSet : VideoIngestFactoryTests
        {
            [SetUp]
            public void BeforeEach()
            {
                FileModel = new ObjectFileModel(GoodFileName);
                Provenance = GenerateFileProvenance(GoodFileName);

                Provenance.DateDigitized = null;

                PodMetadata = new VideoPodMetadata
                {
                    Format = "8mm Video",
                    FileProvenances = new List<AbstractDigitalFile> {Provenance}
                };
            }

            [Test]
            public void ShouldThrowOutputXmlException()
            {
                var factory = new IngestDataFactory();
                Assert.Throws<OutputXmlException>(() => factory.Generate(PodMetadata, FileModel));
            }
        }

        public class WhenProvenanceIsMissing : VideoIngestFactoryTests
        {
            [SetUp]
            public void BeforeEach()
            {
                FileModel = new ObjectFileModel(MissingFileName);
                Provenance = GenerateFileProvenance(GoodFileName);

                PodMetadata = new VideoPodMetadata
                {
                    Format = "8mm Video",
                    FileProvenances = new List<AbstractDigitalFile> {Provenance}
                };
            }

            [Test]
            public void ShouldThrowOutputXmlException()
            {
                var factory = new IngestDataFactory();
                Assert.Throws<OutputXmlException>(() => factory.Generate(PodMetadata, FileModel));
            }
        }

        public class WhenProvenanceIsPresent : VideoIngestFactoryTests
        {
            [SetUp]
            public void BeforeEach()
            {
                FileModel = new ObjectFileModel(GoodFileName);
                Provenance = GenerateFileProvenance(GoodFileName);

                PodMetadata = new VideoPodMetadata
                {
                    Format = "8mm video",
                    FileProvenances = new List<AbstractDigitalFile> {Provenance}
                };

                var factory = new IngestDataFactory();
                Result = factory.Generate(PodMetadata, FileModel);
            }

            private static void AssertDeviceCollectionOk(IReadOnlyList<Device> devices,
                IReadOnlyList<Packager.Models.OutputModels.Ingest.Device> ingestDevices)
            {
                Assert.That(devices.Count, Is.EqualTo(ingestDevices.Count));
                for (var i = 0; i < devices.Count; i++)
                {
                    Assert.That(ingestDevices[i].Model, Is.EqualTo(devices[i].Model));
                    Assert.That(ingestDevices[i].SerialNumber, Is.EqualTo(devices[i].SerialNumber));
                    Assert.That(ingestDevices[i].Manufacturer, Is.EqualTo(devices[i].Manufacturer));
                }
            }

            [Test]
            public void ItShouldExtractionWorkstationCorrectly()
            {
                Assert.That(Result.ExtractionWorkstation.SerialNumber,
                    Is.EqualTo(Provenance.ExtractionWorkstation.SerialNumber));
                Assert.That(Result.ExtractionWorkstation.Model, Is.EqualTo(Provenance.ExtractionWorkstation.Model));
                Assert.That(Result.ExtractionWorkstation.Manufacturer,
                    Is.EqualTo(Provenance.ExtractionWorkstation.Manufacturer));
            }

            [Test]
            public void ItShouldPlayerDevicesCorrectly()
            {
                AssertDeviceCollectionOk(Provenance.PlayerDevices.ToArray(), Result.Players);
            }

            [Test]
            public void ItShouldSetAdDevicesCorrectly()
            {
                AssertDeviceCollectionOk(Provenance.AdDevices.ToArray(), Result.AdDevices);
            }

            [Test]
            public void ItShouldSetCommentsCorrectly()
            {
                Assert.That(Result.Comments, Is.EqualTo(Provenance.Comment));
            }

            [Test]
            public void ItShouldSetCreatedByCorrectly()
            {
                Assert.That(Result.CreatedBy, Is.EqualTo(Provenance.CreatedBy));
            }

            [Test]
            public void ItShouldSetDateCorrectly()
            {
                Assert.That(Result.Date, Is.EqualTo(Provenance.DateDigitized.Value.ToString("yyyy-MM-dd")));
            }

            [Test]
            public void ItShouldSetDigitStatusCorrectly()
            {
                Assert.That(Result.DigitStatus, Is.EqualTo("OK"));
            }

            [Test]
            public void ItShouldSetEncoderCorrectly()
            {
                Assert.That(Result.Encoder.SerialNumber,
                    Is.EqualTo(Provenance.Encoder.SerialNumber));
                Assert.That(Result.Encoder.Model, Is.EqualTo(Provenance.Encoder.Model));
                Assert.That(Result.Encoder.Manufacturer,
                    Is.EqualTo(Provenance.Encoder.Manufacturer));
            }

            [Test]
            public void ItShouldSetTBCDevicesCorrectly()
            {
                AssertDeviceCollectionOk(Provenance.TBCDevices.ToArray(), Result.TbcDevices);
            }
        }
    }
}