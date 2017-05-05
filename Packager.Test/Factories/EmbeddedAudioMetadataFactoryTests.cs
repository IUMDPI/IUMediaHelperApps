using System;
using System.Collections.Generic;
using System.IO;
using Common.Models;
using NSubstitute;
using NUnit.Framework;
using Packager.Exceptions;
using Packager.Factories;
using Packager.Factories.CodingHistory;
using Packager.Models.EmbeddedMetadataModels;
using Packager.Models.FileModels;
using Packager.Models.PodMetadataModels;
using Packager.Models.SettingsModels;

namespace Packager.Test.Factories
{
    [TestFixture]
    public class EmbeddedAudioMetadataFactoryTests
    {
        [SetUp]
        public void BeforeEach()
        {
            CodingHistoryGenerator = Substitute.For<ICodingHistoryGenerator>();
            ProgramSettings = Substitute.For<IProgramSettings>();
            ProgramSettings.DigitizingEntity.Returns(DigitizingEntity);
            Provenance = GetFileProvenance();
            CodingHistoryGenerators = new Dictionary<IMediaFormat, ICodingHistoryGenerator>
            {
                {MediaFormats.OpenReelAudioTape, CodingHistoryGenerator }
            };
           
            Model = FileModelFactory.GetModel(PreservationFileName);
            Instances = new List<AbstractFile> {Model};
            Metadata = new AudioPodMetadata
            {
                Unit = Unit,
                CallNumber = CallNumber,
                Format = MediaFormats.OpenReelAudioTape,
                Title = Title,
                SoundField = "Mono",
                PlaybackSpeed = "7.5 ips",
                FileProvenances = new List<AbstractDigitalFile> {Provenance},
                Iarl = Iarl,
                Bext = Bext,
            };

            Factory = new EmbeddedAudioMetadataFactory(ProgramSettings, CodingHistoryGenerators);
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
                BextFile = BextFile
            };
        }

        private const string PreservationFileName = "MDPI_4890764553278906_01_pres.wav";
        private const string DigitizingEntity = "Test digitizing entity";
        private const string Unit = "Test unit";
        private const string CallNumber = "AB1243";
        private const string Title = "Test title";
        private const string Iarl = "Indiana University-Bloomington. Office of University Archives and Records Management";
        private const string Bext = "Indiana University-Bloomington. Office of University Archives and Records Management. isos048. File use:";
        private const string BextFile = "Indiana University-Bloomington. Office of University Archives and Records Management. isos048. File use: Preservation Master. MDPI_4890764553278906_01_pres";
        private IProgramSettings ProgramSettings { get; set; }
        private Dictionary<IMediaFormat, ICodingHistoryGenerator> CodingHistoryGenerators { get; set; }
       
        private AbstractFile Model { get; set; }
        private DigitalAudioFile Provenance { get; set; }
        private AudioPodMetadata Metadata { get; set; }
        private ICodingHistoryGenerator CodingHistoryGenerator { get; set; }
        private List<AbstractFile> Instances { get; set; }
        private EmbeddedAudioMetadataFactory Factory { get; set; }


        [Test]
        public void InamShouldBeSetCorrectly()
        {
            var result = Factory.Generate(Instances, Model, Metadata) as EmbeddedAudioMetadata;
            Assert.That(result.INAM.Equals(Title));
        }

        [Test]
        public void OrginationTimeShouldBeSetCorrectly()
        {
            var result = Factory.Generate(Instances, Model, Metadata) as EmbeddedAudioMetadata;
            Assert.That(result.OriginationTime.Equals("01:02:03"));
        }

        [Test]
        public void OriginationDateAndIcrdShouldBeSetCorrectly()
        {
            var result = Factory.Generate(Instances, Model, Metadata) as EmbeddedAudioMetadata;
            Assert.That(result.OriginationDate.Equals("2015-08-01"));
            Assert.That(result.ICRD.Equals("2015-08-01"));
        }

        [Test]
        public void OriginatorReferenceShouldBeSetCorrectly()
        {
            var result = Factory.Generate(Instances, Model, Metadata) as EmbeddedAudioMetadata;
            Assert.That(result.OriginatorReference, Is.EqualTo(Path.GetFileNameWithoutExtension(PreservationFileName)));
        }

        [Test]
        public void OriginatorShouldBeSetCorrectly()
        {
            var result = Factory.Generate(Instances, Model, Metadata) as EmbeddedAudioMetadata;
            Assert.That(result.Originator.Equals(DigitizingEntity));
        }

        [Test]
        public void TimeReferenceShouldBeSetCorrectly()
        {
            var result = Factory.Generate(Instances, Model, Metadata) as EmbeddedAudioMetadata;
            Assert.That(result.TimeReference.Equals("0"));
        }

        [Test]
        public void IarlShouldBeSetCorrectly()
        {
            var result = Factory.Generate(Instances, Model, Metadata) as EmbeddedAudioMetadata;
            Assert.That(result.IARL, Is.EqualTo(Iarl));
        }

        [Test]
        public void IcmtAndDescriptionShouldBeCorrect()
        {
          
            var result = Factory.Generate(Instances, Model, Metadata) as EmbeddedAudioMetadata;
            Assert.That(result.ICMT, Is.EqualTo(BextFile));
            Assert.That(result.Description, Is.EqualTo(BextFile));
        }

        [Test]
        public void ItShouldCallCodingHistoryFactoryCorrectly()
        {
            Factory.Generate(Instances, Model, Metadata);
            CodingHistoryGenerator.Received().Generate(Metadata, Provenance, Model);
        }

        [Test]
        public void ItShouldThrowExceptionIfFormatNotSupported()
        {
            Metadata.Format = new UnknownMediaFormat("unknown format");
            var exception = Assert.Throws<EmbeddedMetadataException>(() => Factory.Generate(Instances, Model, Metadata));
            Assert.That(exception.Message, Is.EqualTo($"No coding history generator defined for {Metadata.Format}"));
        }
    }
}