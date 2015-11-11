using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Packager.Exceptions;
using Packager.Factories;
using Packager.Models.FileModels;
using Packager.Models.OutputModels;
using Packager.Models.PodMetadataModels;
using Packager.Models.PodMetadataModels.ConsolidatedModels;
using Device = Packager.Models.PodMetadataModels.ConsolidatedModels.Device;

namespace Packager.Test.Factories
{
    [TestFixture]
    public class IngestFactoryTests
    {
        private const string GoodFileName = "MDPI_4890764553278906_01_pres.wav";
        private const string MissingFileName = "MDPI_111111111111_01_pres.wav";
        private ConsolidatedAudioPodMetadata PodMetadata { get; set; }
        private AbstractFileModel FileModel { get; set; }
        private ConsolidatedDigitalAudioFile Provenance { get; set; }
        private IngestData Result { get; set; }

        private ConsolidatedDigitalAudioFile GenerateFileProvenance(string fileName)
        {
            var original = new DigitalFileProvenance
            {
                Comment = "Comment",
                CreatedBy = "Created by",
                DateDigitized = new DateTime(2015, 8, 1, 1, 2, 3),
                Filename = fileName,
                //SignalChain = GetSignalChain(),
                SpeedUsed = "7.5 ips"
            };


            return new ConsolidatedDigitalAudioFile(original);
        }
        
        private static List<Device> GetSignalChain()
        {
            var result = new List<Device>
            {
                new Device {DeviceType = "extraction workstation", Model = "ew model", Manufacturer = "ew manufacturer", SerialNumber = "ew serial number"},
                new Device {DeviceType = "player", Model = "Player model", Manufacturer = "Player manufacturer", SerialNumber = "Player serial number"},
                new Device {DeviceType = "ad", Model = "Ad model", Manufacturer = "Ad manufacturer", SerialNumber = "Ad serial number"},
            };

            return result;
        }

        public class WhenProvenanceIsMissing : IngestFactoryTests
        {
            [SetUp]
            public void BeforeEach()
            {
                FileModel = new ObjectFileModel(MissingFileName);
                Provenance = GenerateFileProvenance(GoodFileName);

                PodMetadata = new ConsolidatedAudioPodMetadata
                {
                    Format = "CD-R",
                    FileProvenances = new List<AbstractConsolidatedDigitalFile> {Provenance}
                };
            }

            [Test]
            public void ShouldThrowOutputXmlException()
            {
                var factory = new IngestDataFactory();
                Assert.Throws<OutputXmlException>(() => factory.Generate(PodMetadata, FileModel));
            }
        }

        public class WhenProvenanceIsPresent : IngestFactoryTests
        {
            [SetUp]
            public void BeforeEach()
            {
                FileModel = new ObjectFileModel(GoodFileName);
                Provenance = GenerateFileProvenance(GoodFileName);

                PodMetadata = new ConsolidatedAudioPodMetadata
                {
                    Format = "CD-R",
                    FileProvenances = new List<AbstractConsolidatedDigitalFile> {Provenance}
                };

                var factory = new IngestDataFactory();
                Result = factory.Generate(PodMetadata, FileModel);
            }

            private static void AssertDeviceCollectionOk(IReadOnlyList<Device> devices, IReadOnlyList<IngestDevice> ingestDevices)
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
                Assert.That(Result.ExtractionWorkstation.SerialNumber, Is.EqualTo(Provenance.ExtractionWorkstation.SerialNumber));
                Assert.That(Result.ExtractionWorkstation.Model, Is.EqualTo(Provenance.ExtractionWorkstation.Model));
                Assert.That(Result.ExtractionWorkstation.Manufacturer, Is.EqualTo(Provenance.ExtractionWorkstation.Manufacturer));
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
                Assert.That(Result.Date, Is.EqualTo(Provenance.DateDigitized.Value.ToString()));
            }

            [Test]
            public void ItShouldSetSpeedUsedCorrectly()
            {
                Assert.That(Result.SpeedUsed, Is.EqualTo(Provenance.SpeedUsed));
            }

            [Test]
            public void ItShouldSetXsiTypeCorrectly()
            {
                Assert.That(Result.XsiType, Is.EqualTo($"{PodMetadata.Format}Ingest"));
            }
        }
    }
}