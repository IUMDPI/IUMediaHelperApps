using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using NUnit.Framework;
using Packager.Factories;
using Packager.Models.EmbeddedMetadataModels;
using Packager.Models.FileModels;
using Packager.Models.PodMetadataModels;
using Device = Packager.Models.PodMetadataModels.Device;

namespace Packager.Test.Factories
{
    [TestFixture]
    public class ConformancePointDocumentFactoryTests
    {
       private const string PreservationFileName = "MDPI_4890764553278906_01_pres.wav";
        private const string DigitizingEntity = "Test digitizing entity";
        private const string Unit = "Test unit";
        private const string CallNumber = "AB1243";
        private const string Title = "Test title";

        private EmbeddedAudioMetadata Result { get; set; }
        private ObjectFileModel Model { get; set; }
        private DigitalAudioFile Provenance { get; set; }
        private AudioPodMetadata Metadata { get; set; }

        private List<ObjectFileModel> Instances { get; set; } 

        private string ExpectedDigitalOrAnalog { get; set; }

        protected virtual void DoCustomSetup()
        {
            ExpectedDigitalOrAnalog = "ANALOGUE";
        }

        [SetUp]
        public void BeforeEach()
        {
            Provenance = GetFileProvenance();
            Model = new ObjectFileModel(PreservationFileName);
            Instances = new List<ObjectFileModel> {Model};
            Metadata = new AudioPodMetadata
            {
                DigitizingEntity = DigitizingEntity,
                Unit = Unit,
                CallNumber = CallNumber,
                Format = "Record",
                Title = Title,
                SoundField = "Mono",
                PlaybackSpeed = "7.5 ips",
                FileProvenances = new List<AbstractDigitalFile> { Provenance}
            };

            DoCustomSetup();

            Result = new EmbeddedAudioMetadataFactory().Generate(Instances, Model, Metadata) as EmbeddedAudioMetadata;
        }

        private static DigitalAudioFile GetFileProvenance()
        {
    
            return new DigitalAudioFile
            {
                Comment = "Comment",
                CreatedBy = "Created by",
                DateDigitized = new DateTime(2015, 8, 1, 1, 2, 3),
                Filename = PreservationFileName,
                SpeedUsed = "7.5 ips",
                SignalChain = GetSignalChain()
            };
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

        [Test]
        public void TimeReferenceShouldBeSetCorrectly()
        {
            Assert.That(Result.TimeReference.Equals("0"));
        }

        [Test]
        public void InamShouldBeSetCorrectly()
        {
            Assert.That(Result.INAM.Equals(Title));
        }

        [Test]
        public void IarlShouldBeSetCorrectly()
        {
            Assert.That(Result.IARL.Equals(Unit));
        }

        [Test]
        public void OriginationDateAndIcrdShouldBeSetCorrectly()
        {
            Assert.That(Result.OriginationDate.Equals("2015-08-01"));
            Assert.That(Result.ICRD.Equals("2015-08-01"));
        }

        [Test]
        public void OrginationTimeShouldBeSetCorrectly()
        {
            Assert.That(Result.OriginationTime.Equals("01:02:03"));
        }

        [Test]
        public void DescriptionAndIcmtShouldBeSetCorrectly()
        {
            var expected = $"{Unit}. {CallNumber}. File use: {Model.FullFileUse}. {Path.GetFileNameWithoutExtension(PreservationFileName)}";
            Assert.That(Result.Description, Is.EqualTo(expected));
            Assert.That(Result.ICMT, Is.EqualTo(expected));
        }

        [Test]
        public void OriginatorReferenceShouldBeSetCorrectly()
        {
            Assert.That(Result.OriginatorReference, Is.EqualTo(Path.GetFileNameWithoutExtension(PreservationFileName)));
        }

        [Test]
        public void OriginatorShouldBeSetCorrectly()
        {
            Assert.That(Result.Originator.Equals(DigitizingEntity));
        }

        [Test]
        public void CodingHistoryShouldHaveThreeLines()
        {
            var parts = Result.CodingHistory.Split(new [] { "\r\n" }, StringSplitOptions.None);
            Assert.That(parts.Length, Is.EqualTo(3));
        }
        
        [Test]
        public void CodingHistoryLine2ShouldBeCorrect()
        {
            var parts = Result.CodingHistory.Split(new[] { "\r\n" }, StringSplitOptions.None);

            const string expected = "A=PCM,F=96000,W=24,M=Mono,T=Ad manufacturer Ad model;SNAd serial number;A/D,";

            Assert.That(parts[1], Is.EqualTo(expected));
        }

        [Test]
        public void CodingHistoryLine3ShouldBeCorrect()
        {
            var parts = Result.CodingHistory.Split(new[] { "\r\n" }, StringSplitOptions.None);

            const string expected = "A=PCM,F=96000,W=24,M=mono,T=Lynx AES16;DIO";

            // kludge: trimming possible final space to account for padding
            // that might be added to fix issues that break AudioInspector and
            // BatchInspector
            Assert.That(parts[2].Trim(), Is.EqualTo(expected));
        }

        [TestFixture]
        public class WhenFormatIsCDR : ConformancePointDocumentFactoryTests
        {
            protected override void DoCustomSetup()
            {
                base.DoCustomSetup();
                Metadata.Format = "CD-R";
                ExpectedDigitalOrAnalog = "DIGITAL";
            }
        }

        [TestFixture]
        public class WhenFormatIsDAT : ConformancePointDocumentFactoryTests
        {
            protected override void DoCustomSetup()
            {
                base.DoCustomSetup();
                Metadata.Format = "DAT";
                ExpectedDigitalOrAnalog = "DIGITAL";
            }
        }

        [TestFixture]
        public class WhenSpeedIsSetInProvenance : ConformancePointDocumentFactoryTests
        {
            protected override void DoCustomSetup()
            {
                base.DoCustomSetup();
                Provenance.SpeedUsed = "8.5 ips";
            }

            [Test]
            public void ItShouldUseProvenanceSpeed()
            {
                var parts = Result.CodingHistory.Split(new[] { "\r\n" }, StringSplitOptions.None);

                var expected = $"A={ExpectedDigitalOrAnalog}," + "M=Mono," + $"T=Player manufacturer Player model;SNPlayer serial number;8.5 ips;{Metadata.Format},";

                Assert.That(parts[0], Is.EqualTo(expected));
            }
        }

        [TestFixture]
        public class WhenMultipleSpeedsSetInProvenance : ConformancePointDocumentFactoryTests
        {
            protected override void DoCustomSetup()
            {
                base.DoCustomSetup();
                Provenance.SpeedUsed = "8.5 ips, 1.1 ips, 2.1 ips";
            }

            [Test]
            public void ItShouldRenderSpeedsCorrectly()
            {
                var parts = Result.CodingHistory.Split(new[] { "\r\n" }, StringSplitOptions.None);

                var expected = $"A={ExpectedDigitalOrAnalog}," + "M=Mono," + $"T=Player manufacturer Player model;SNPlayer serial number;8.5 ips;1.1 ips;2.1 ips;{Metadata.Format},";

                Assert.That(parts[0], Is.EqualTo(expected));
            }
        }

        [TestFixture]
        public class WhenSpeedNotSetInProvenance : ConformancePointDocumentFactoryTests
        {
            protected override void DoCustomSetup()
            {
                base.DoCustomSetup();
                Provenance.SpeedUsed = "";
            }

            [Test]
            public void ItShouldUseMetadataPlaybackSpeed()
            {
                var parts = Result.CodingHistory.Split(new[] { "\r\n" }, StringSplitOptions.None);

                var expected = $"A={ExpectedDigitalOrAnalog}," + "M=Mono," + $"T=Player manufacturer Player model;SNPlayer serial number;{Metadata.PlaybackSpeed};{Metadata.Format},";

                Assert.That(parts[0], Is.EqualTo(expected));
            }
        }
        
        [TestFixture]
        public class WhenSpeedMissing : ConformancePointDocumentFactoryTests
        {
            protected override void DoCustomSetup()
            {
                base.DoCustomSetup();
                Provenance.SpeedUsed = "";
                Metadata.PlaybackSpeed = "";
            }

            [Test]
            public void ItShouldNotRenderPlaybackSpeed()
            {
                var parts = Result.CodingHistory.Split(new[] { "\r\n" }, StringSplitOptions.None);

                var expected = $"A={ExpectedDigitalOrAnalog}," + "M=Mono," + $"T=Player manufacturer Player model;SNPlayer serial number;{Metadata.Format},";

                Assert.That(parts[0], Is.EqualTo(expected));
            }
        }



    }
}