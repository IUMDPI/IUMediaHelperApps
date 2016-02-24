using System;
using System.Collections.Generic;
using System.IO;
using NSubstitute;
using NUnit.Framework;
using Packager.Factories;
using Packager.Models.EmbeddedMetadataModels;
using Packager.Models.FileModels;
using Packager.Models.PodMetadataModels;
using Packager.Models.SettingsModels;

namespace Packager.Test.Factories.EmbeddedVideoMetadataFactoryTests
{
    [TestFixture]
    public abstract class EmbeddedVideoMetadataFactoryTests
    {
        [SetUp]
        public void BeforeEach()
        {
            ProgramSettings = Substitute.For<IProgramSettings>();
            ProgramSettings.DigitizingEntity.Returns(DigitizingEntity);
            PresProvenance = GetFileProvenance(PresFilename);
            MezzProvenance = GetFileProvenance(MezzFilename);
            PresModel = FileModelFactory.GetModel(PresFilename);
            MezzModel = FileModelFactory.GetModel(MezzFilename);
            Instances = new List<AbstractFile> {PresModel, MezzModel};
            Metadata = new VideoPodMetadata
            {
                Unit = Unit,
                CallNumber = CallNumber,
                Format = "Record",
                Title = Title,
                Iarl = Iarl,
                Bext = Bext,
                FileProvenances = new List<AbstractDigitalFile> {PresProvenance, MezzProvenance}
            };

            DoCustomSetup();

            Result = new EmbeddedVideoMetadataFactory(ProgramSettings)
                .Generate(Instances, ModelToUseForTesting, Metadata) as AbstractEmbeddedVideoMetadata;
        }

        private const string PresFilename = "MDPI_4890764553278906_01_pres.mkv";
        private const string MezzFilename = "MDPI_4890764553278906_01_mezz.mov";
        private const string DigitizingEntity = "Test digitizing entity";
        private const string Unit = "Test unit";
        private const string CallNumber = "AB1243";
        private const string Title = "Test title";
        private const string Iarl = "Indiana University-Bloomington. Office of University Archives and Records Management";
        private const string Bext = "Indiana University-Bloomington. Office of University Archives and Records Management. isos048. File use:";

        private string FilenameToUseForTesting { get; set; }
        private AbstractFile ModelToUseForTesting { get; set; }
        private AbstractEmbeddedVideoMetadata Result { get; set; }
        private AbstractFile PresModel { get; set; }
        private AbstractFile MezzModel { get; set; }
        private DigitalVideoFile PresProvenance { get; set; }
        private DigitalVideoFile MezzProvenance { get; set; }
        private VideoPodMetadata Metadata { get; set; }
        private List<AbstractFile> Instances { get; set; }
        private IProgramSettings ProgramSettings { get; set; }
        protected virtual void DoCustomSetup()
        {
        }

        private static DigitalVideoFile GetFileProvenance(string filename)
        {
            return new DigitalVideoFile
            {
                Comment = "Comment",
                CreatedBy = "Created by",
                DateDigitized = new DateTime(2015, 8, 1, 1, 2, 3),
                Filename = filename,
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
                }
            };

            return result;
        }

        [Test]
        public void DescriptionShouldBeCorrect()
        {
            Assert.That(Result.Description, Is.EqualTo($"{Bext} {ModelToUseForTesting.FullFileUse}. {Path.GetFileNameWithoutExtension(ModelToUseForTesting.Filename)}"));
        }

        [Test]
        public void TitleShouldBeCorrect()
        {
            Assert.That(Result.Title, Is.EqualTo(Metadata.Title));
        }

        [Test]
        public void MasteredDateShouldBeCorrect()
        {
            Assert.That(Result.MasteredDate, Is.EqualTo("2015-08-01"));
        }

        [Test]
        public void CommentShouldBeCorrect()
        {
            Assert.That(Result.Comment, Is.EqualTo($"File created by {DigitizingEntity}"));
        }

        public class WhenModelIsPreservationMaster : EmbeddedVideoMetadataFactoryTests
        {
            protected override void DoCustomSetup()
            {
                base.DoCustomSetup();
                FilenameToUseForTesting = PresFilename;
                ModelToUseForTesting = PresModel;
            }

            [Test]
            public void ResultShouldBeCorrectType()
            {
                Assert.That(Result is EmbeddedVideoPreservationMetadata);
            }
        }

        public class WhenModelIsMezzanine : EmbeddedVideoMetadataFactoryTests
        {
            protected override void DoCustomSetup()
            {
                base.DoCustomSetup();
                FilenameToUseForTesting = MezzFilename;
                ModelToUseForTesting = MezzModel;
            }

            [Test]
            public void ResultShouldBeCorrectType()
            {
                Assert.That(Result is EmbeddedVideoMezzanineMetadata);
            }
        }
    }
}