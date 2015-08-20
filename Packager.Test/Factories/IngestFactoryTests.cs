using System;
using System.Collections.Generic;
using System.Globalization;
using NUnit.Framework;
using Packager.Exceptions;
using Packager.Factories;
using Packager.Models.FileModels;
using Packager.Models.OutputModels;
using Packager.Models.PodMetadataModels;

namespace Packager.Test.Factories
{
    [TestFixture]
    public class IngestFactoryTests
    {
        private const string GoodFileName = "MDPI_4890764553278906_01_pres.wav";
        private const string MissingFileName = "MDPI_111111111111_01_pres.wav";
        private ConsolidatedPodMetadata PodMetadata { get; set; }
        private AbstractFileModel FileModel { get; set; }
        private DigitalFileProvenance Provenance { get; set; }
        private IngestData Result { get; set; }

        private DigitalFileProvenance GenerateFileProvenance(string fileName)
        {
            return new DigitalFileProvenance
            {
                Filename = fileName,
                AdManufacturer = "Ad Manufacturer",
                AdModel = "Ad Model",
                AdSerialNumber = "Ad Serial Number",
                Comment = "File provenance comment",
                CreatedAt = new DateTime(2015, 05, 01).ToString(CultureInfo.InvariantCulture),
                CreatedBy = "Test user",
                DateDigitized = new DateTime(2015, 05, 01),
                ExtractionWorkstation = "Extraction workstation",
                PlayerManufacturer = "Player manufacturer",
                PlayerModel = "Player model",
                PlayerSerialNumber = "Player serial number",
                SpeedUsed = "7.5 ips",
                UpdatedAt = new DateTime(2015, 05, 01).ToString(CultureInfo.InvariantCulture),
                PreAmp = "Pre amp",
                PreAmpSerialNumber = "Pre amp serial number"
            };
        }

       
        public class WhenProvenanceIsMissing : IngestFactoryTests
        {
            [SetUp]
            public void BeforeEach()
            {

                FileModel = new ObjectFileModel(MissingFileName);
                Provenance = GenerateFileProvenance(GoodFileName);

                PodMetadata = new ConsolidatedPodMetadata
                {
                    Format = "CD-R",
                    FileProvenances = new List<DigitalFileProvenance> { Provenance }
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

                PodMetadata = new ConsolidatedPodMetadata
                {
                    Format = "CD-R",
                    FileProvenances = new List<DigitalFileProvenance> { Provenance }
                };

                var factory = new IngestDataFactory();
                Result = factory.Generate(PodMetadata, FileModel);
            }
            
            [Test]
            public void ItShouldSetXsiTypeCorrectly()
            {
                Assert.That(Result.XsiType, Is.EqualTo(string.Format("{0}Ingest", PodMetadata.Format)));
            }

            [Test]
            public void ItShouldSetAdManufacturerCorrectly()
            {
                Assert.That(Result.AdManufacturer, Is.EqualTo(Provenance.AdManufacturer));
            }

            [Test]
            public void ItShouldSetAdModelCorrectly()
            {
                Assert.That(Result.AdModel, Is.EqualTo(Provenance.AdModel));
            }

            [Test]
            public void ItShouldSetAdSerialNumberCorrectly()
            {
                Assert.That(Result.AdSerialNumber, Is.EqualTo(Provenance.AdSerialNumber));
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
            public void ItShouldSetPlayerManufacturerCorrectly()
            {
                Assert.That(Result.PlayerManufacturer, Is.EqualTo(Provenance.PlayerManufacturer));
            }

            [Test]
            public void ItShouldSetPlayerModelCorrectly()
            {
                Assert.That(Result.PlayerModel, Is.EqualTo(Provenance.PlayerModel));
            }

            [Test]
            public void ItShouldSetPlayerSerialNumberCorrectly()
            {
                Assert.That(Result.PlayerSerialNumber, Is.EqualTo(Provenance.PlayerSerialNumber));
            }

            [Test]
            public void ItShouldExtractionWorkstationCorrectly()
            {
                Assert.That(Result.ExtractionWorkstation, Is.EqualTo(Provenance.ExtractionWorkstation));
            }

            [Test]
            public void ItShouldSetSpeedUsedCorrectly()
            {
                Assert.That(Result.SpeedUsed, Is.EqualTo(Provenance.SpeedUsed));
            }

            [Test]
            public void ItShouldSetDateCorrectly()
            {
                Assert.That(Result.Date, Is.EqualTo(Provenance.DateDigitized));
            }

            [Test]
            public void ItShouldSetPreAmpCorrectly()
            {
                Assert.That(Result.PreAmp, Is.EqualTo(Provenance.PreAmp));
            }

            [Test]
            public void ItShouldSetPreAmpSerialNumberCorrectly()
            {
                Assert.That(Result.PreAmpSerialNumber, Is.EqualTo(Provenance.PreAmpSerialNumber));
            }
        }
    }
}