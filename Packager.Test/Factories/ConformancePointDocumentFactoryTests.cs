using System;
using System.Globalization;
using System.IO;
using NUnit.Framework;
using Packager.Factories;
using Packager.Models.BextModels;
using Packager.Models.FileModels;
using Packager.Models.PodMetadataModels;

namespace Packager.Test.Factories
{
    [TestFixture]
    public class ConformancePointDocumentFactoryTests
    {
        private const string BaseProcessDirectory = "base";
        private const string PreservationFileName = "MDPI_4890764553278906_01_pres.wav";
        private const string DigitizingEntity = "Test digitizing entity";
        private const string Unit = "Test unit";
        private const string CallNumber = "AB1243";
        private const string Title = "Test title";

        private ConformancePointDocumentFile Result { get; set; }
        private ObjectFileModel Model { get; set; }
        private DigitalFileProvenance Provenance { get; set; }
        private ConsolidatedPodMetadata Metadata { get; set; }

        private string ExpectedDigitalOrAnalog { get; set; }

        protected virtual void DoCustomSetup()
        {
            ExpectedDigitalOrAnalog = "ANALOG";
        }

        [SetUp]
        public void BeforeEach()
        {
            Provenance = GetFileProvenance();
            Model = new ObjectFileModel(PreservationFileName);

            Metadata = new ConsolidatedPodMetadata
            {
                DigitizingEntity = DigitizingEntity,
                Unit = Unit,
                CallNumber = CallNumber,
                Format = "Record",
                Title = Title,
                SoundField = "Mono",
                PlaybackSpeed = "7.5 ips"
            };

            DoCustomSetup();

            Result = new ConformancePointDocumentFactory(BaseProcessDirectory).Generate(Model, Provenance, Metadata);
        }

        private static DigitalFileProvenance GetFileProvenance()
        {
            return new DigitalFileProvenance
            {
                AdManufacturer = "Ad manufacturer",
                AdModel = "Ad model",
                AdSerialNumber = "Ad serial number",
                Comment = "Comment",
                CreatedAt = new DateTime(2015, 8, 1, 1, 2, 3).ToString(CultureInfo.InvariantCulture),
                CreatedBy = "Created by",
                DateDigitized = new DateTime(2015, 8, 1, 1, 2, 3).ToString(CultureInfo.InvariantCulture),
                ExtractionWorkstation = "Extraction workstation",
                Filename = PreservationFileName,
                PlayerManufacturer = "Player manufacturer",
                PlayerModel = "Player model",
                PlayerSerialNumber = "Player serial number",
                SpeedUsed = "7.5 ips",
                UpdatedAt = new DateTime(2015, 8, 1).ToString(CultureInfo.InvariantCulture)
            };
        }

        [Test]
        public void TimeReferenceShouldBeSetCorrectly()
        {
            Assert.That(Result.Core.TimeReference.Equals("0"));
        }

        [Test]
        public void InamShouldBeSetCorrectly()
        {
            Assert.That(Result.Core.INAM.Equals(Title));
        }

        [Test]
        public void IarlShouldBeSetCorrectly()
        {
            Assert.That(Result.Core.IARL.Equals(Unit));
        }

        [Test]
        public void OriginationDateAndIcrdShouldBeSetCorrectly()
        {
            Assert.That(Result.Core.OriginationDate.Equals("2015-08-01"));
            Assert.That(Result.Core.ICRD.Equals("2015-08-01"));
        }

        [Test]
        public void OrginationTimeShouldBeSetCorrectly()
        {
            Assert.That(Result.Core.OriginationTime.Equals("01:02:03"));
        }

        [Test]
        public void DescriptionAndIcmtShouldBeSetCorrectly()
        {
            var expected = string.Format("{0}. {1}. File use: {2}. {3}", Unit, CallNumber, Model.FullFileUse, Path.GetFileNameWithoutExtension(PreservationFileName));
            Assert.That(Result.Core.Description, Is.EqualTo(expected));
            Assert.That(Result.Core.ICMT, Is.EqualTo(expected));
        }

        [Test]
        public void OriginatorReferenceShouldBeSetCorrectly()
        {
            Assert.That(Result.Core.OriginatorReference, Is.EqualTo(Path.GetFileNameWithoutExtension(PreservationFileName)));
        }

        [Test]
        public void OriginatorShouldBeSetCorrectly()
        {
            Assert.That(Result.Core.Originator.Equals(DigitizingEntity));
        }

        [Test]
        public void CodingHistoryShouldHaveThreeLines()
        {
            var parts = Result.Core.CodingHistory.Split(new [] { "\r\n" }, StringSplitOptions.None);
            Assert.That(parts.Length, Is.EqualTo(3));
        }

        [Test]
        public void CodingHistoryLine1ShouldBeCorrect()
        {
            var parts = Result.Core.CodingHistory.Split(new[] { "\r\n" }, StringSplitOptions.None);

            var expected = string.Format("A={0}," +
                                         "M=Mono," +
                                         "T=Player manufacturer Player model;Player serial number;7.5 ips;{1},",
                ExpectedDigitalOrAnalog, Metadata.Format);

            Assert.That(parts[0], Is.EqualTo(expected));
        }

        [Test]
        public void CodingHistoryLine2ShouldBeCorrect()
        {
            var parts = Result.Core.CodingHistory.Split(new[] { "\r\n" }, StringSplitOptions.None);

            const string expected = "A=PCM,F=96000,W=24,M=Mono,T=Ad manufacturer Ad model;Ad serial number;A/D,";

            Assert.That(parts[1], Is.EqualTo(expected));
        }

        [Test]
        public void CodingHistoryLine3ShouldBeCorrect()
        {
            var parts = Result.Core.CodingHistory.Split(new[] { "\r\n" }, StringSplitOptions.None);

            const string expected = "A=PCM,F=96000,W=24,M=mono,T=Lynx AES16;DIO";

            Assert.That(parts[2], Is.EqualTo(expected));
        }


        public class WhenFormatIsCDR : ConformancePointDocumentFactoryTests
        {
            protected override void DoCustomSetup()
            {
                base.DoCustomSetup();
                Metadata.Format = "CD-R";
                ExpectedDigitalOrAnalog = "DIGITAL";
            }
        }

        public class WhenFormatIsDAT : ConformancePointDocumentFactoryTests
        {
            protected override void DoCustomSetup()
            {
                base.DoCustomSetup();
                Metadata.Format = "DAT";
                ExpectedDigitalOrAnalog = "DIGITAL";
            }


        }



    }
}