using Common.Models;
using NUnit.Framework;
using Packager.Exceptions;
using Packager.Factories.CodingHistory;
using Packager.Models.FileModels;
using Packager.Models.PodMetadataModels;
using System;

namespace Packager.Test.Factories.CodingHistory
{
    [TestFixture]
    public class SeventyEightCodingHistoryGeneratorTests : CodingHistoryGeneratorTestsBase
    {
        [SetUp]
        public override void BeforeEach()
        {
            base.BeforeEach();

            Metadata = new AudioPodMetadata
            {
                Format = MediaFormats.OpenReelAudioTape
            };

            Generator = new StandardCodingHistoryGenerator();
        }

        private static readonly object[] _lines2And3Cases =
        {
            new object[] {PresModel, "Stereo" },
            new object[] {PresIntModel, "Stereo" },
            new object[] {ProdModel,  "Mono" },
        };

        private static readonly object[] _line1Cases =
        {
            new object[] {PresModel, "8.5 ips", "Mono", "8.5 ips;" },
            new object[] {PresModel, "8.5 ips, 1.1 ips, 2.1 ips", "Mono", "8.5 ips;1.1 ips;2.1 ips;" },
            new object[] {PresModel, null, "Mono", "" },
            new object[] {PresModel, "", "Mono", "" },
            new object[] {PresModel, "8.5 ips", "Stereo", "8.5 ips;" },
            new object[] {PresModel, "8.5 ips, 1.1 ips, 2.1 ips", "Stereo", "8.5 ips;1.1 ips;2.1 ips;" },
            new object[] {PresModel, null, "Stereo", "" },
            new object[] {PresModel, "", "Stereo", "" },

            new object[] {PresIntModel, "8.5 ips", "Mono", "8.5 ips;"  },
            new object[] {PresIntModel, "8.5 ips, 1.1 ips, 2.1 ips", "Mono", "8.5 ips;1.1 ips;2.1 ips;" },
            new object[] {PresIntModel, null, "Mono", "" },
            new object[] {PresIntModel, "", "Mono", "" },
            new object[] {PresIntModel, "8.5 ips", "Stereo", "8.5 ips;"  },
            new object[] {PresIntModel, "8.5 ips, 1.1 ips, 2.1 ips", "Stereo", "8.5 ips;1.1 ips;2.1 ips;" },
            new object[] {PresIntModel, null, "Stereo", "" },
            new object[] {PresIntModel, "", "Stereo", "" },

            new object[] {ProdModel, "8.5 ips", "Mono", "8.5 ips;" },
            new object[] {ProdModel, "8.5 ips, 1.1 ips, 2.1 ips", "Mono", "8.5 ips;1.1 ips;2.1 ips;" },
            new object[] {ProdModel, null, "Mono", "" },
            new object[] {ProdModel, "", "Mono", "" },
            new object[] {ProdModel, "8.5 ips", "Stereo", "8.5 ips;" },
            new object[] {ProdModel, "8.5 ips, 1.1 ips, 2.1 ips", "Stereo", "8.5 ips;1.1 ips;2.1 ips;" },
            new object[] {ProdModel, null, "Stereo", "" },
            new object[] {ProdModel, "", "Stereo", "" },
        };

        [Test, TestCaseSource(nameof(_line1Cases))]
        public void Line1ShouldBeCorrect(AbstractFile model, string speed, string soundField, string expectedSpeedText)
        {
            Metadata.SoundField = soundField;
            Provenance.SpeedUsed = speed;

            var result = Generator.Generate(Metadata, Provenance, model);
            var parts = result.Split(new[] { "\r\n" }, StringSplitOptions.None);

            var expected =
                $"A=ANALOGUE,M={soundField},T=Player manufacturer Player model;SNPlayer serial number;{expectedSpeedText}Open Reel Audio Tape,";

            Assert.That(parts[0], Is.EqualTo(expected));
        }

        [Test, TestCaseSource(nameof(_lines2And3Cases))]
        public void Line2ShouldBeCorrect(AbstractFile model, string soundField)
        {
            Metadata.SoundField = soundField;

            var result = Generator.Generate(Metadata, Provenance, model);
            var parts = result.Split(new[] { "\r\n" }, StringSplitOptions.None);

            var expected = $"A=PCM,F=96000,W=24,M={soundField},T=Ad manufacturer Ad model;SNAd serial number;A/D,";
            Assert.That(parts[1], Is.EqualTo(expected));
        }

        [Test, TestCaseSource(nameof(_lines2And3Cases))]
        public void Line3ShouldBeCorrect(AbstractFile model, string soundField)
        {
            Metadata.SoundField = soundField;

            var result = Generator.Generate(Metadata, Provenance, model);
            var parts = result.Split(new[] { "\r\n" }, StringSplitOptions.None);

            var expected = $"A=PCM,F=96000,W=24,M={Metadata.SoundField},T=Lynx AES16;DIO";
            Assert.That(parts[2], Is.EqualTo(expected));
        }

        [Test]
        public void ItShouldThrowExceptionIfSoundFieldNotSetInMetadata()
        {
            Metadata.SoundField = null;
            var exception = Assert.Throws<EmbeddedMetadataException>(
                () => Generator.Generate(Metadata, Provenance, PresModel));
            Assert.That(exception.Message, Is.EqualTo("No sound field specified in metadata"));
        }

    }
}

