using Common.Models;
using NUnit.Framework;
using Packager.Exceptions;
using Packager.Extensions;
using Packager.Factories.CodingHistory;
using Packager.Models.FileModels;
using Packager.Models.PodMetadataModels;
using System;

namespace Packager.Test.Factories.CodingHistory
{
    public class DatCodingHistoryGeneratorTests : CodingHistoryGeneratorTestsBase
    {
        [SetUp]
        public override void BeforeEach()
        {
            base.BeforeEach();

            Metadata = new AudioPodMetadata
            {
                Format = MediaFormats.Dat
            };
            
            Generator = new DatCodingHistoryGenerator();
        }

        private static readonly object[] _lines2And3Cases =
       {
            new object[] {PresModel, "32K", "32000" },
            new object[] {PresModel, "44.1K", "44100" },
            new object[] {PresModel, "48K", "48000" },
            new object[] {PresModel, "96k", "96000" }, // lower case on purpose to test Dictionary

            new object[] {PresIntModel, "32K", "32000" },
            new object[] {PresIntModel, "44.1K", "44100" },
            new object[] {PresIntModel, "48K", "48000" },
            new object[] {PresIntModel, "96k", "96000" },
            
            new object[] {ProdModel, "32K", "32000" },
            new object[] {ProdModel, "44.1K", "44100" },
            new object[] {ProdModel, "48K", "48000" },
            new object[] {ProdModel, "96k", "96000" }
        };

        // test case variables for line one of the coding history
        private static readonly object[] _line1Cases =
        {
            new object[] {PresModel, "8.5 ips", "8.5 ips;", true },
            new object[] {PresModel, "8.5 ips", "8.5 ips;", false },
            new object[] {PresModel, "8.5 ips, 1.1 ips, 2.1 ips", "8.5 ips;1.1 ips;2.1 ips;", false },
            new object[] {PresModel, "8.5 ips, 1.1 ips, 2.1 ips", "8.5 ips;1.1 ips;2.1 ips;", true },
            
            new object[] {PresModel, null, "", true },
            new object[] {PresModel, null, "", false },

            new object[] {PresModel, "", "", true },
            new object[] {PresModel, "", "", false },

            new object[] {PresIntModel, "8.5 ips", "8.5 ips;", true  },
            new object[] {PresIntModel, "8.5 ips", "8.5 ips;", false  },

            new object[] {PresIntModel, "8.5 ips, 1.1 ips, 2.1 ips", "8.5 ips;1.1 ips;2.1 ips;", true },
            new object[] {PresIntModel, "8.5 ips, 1.1 ips, 2.1 ips", "8.5 ips;1.1 ips;2.1 ips;", false },

            new object[] {PresIntModel, null, "", true },
            new object[] {PresIntModel, null, "", false },

            new object[] {PresIntModel, "", "", true },
            new object[] {PresIntModel, "", "", false },

            new object[] {ProdModel, "8.5 ips", "8.5 ips;", true },
            new object[] {ProdModel, "8.5 ips", "8.5 ips;", false },

            new object[] {ProdModel, "8.5 ips, 1.1 ips, 2.1 ips", "8.5 ips;1.1 ips;2.1 ips;", true },
            new object[] {ProdModel, "8.5 ips, 1.1 ips, 2.1 ips", "8.5 ips;1.1 ips;2.1 ips;", false },

            new object[] {ProdModel, null, "", true },
            new object[] {ProdModel, null, "", false },

            new object[] {ProdModel, "", "", true },
            new object[] {ProdModel, "", "", false },
        };

        [Test, TestCaseSource(nameof(_line1Cases))]
        public void Line1ShouldBeCorrect(AbstractFile model, string speed, string expectedSpeedText, bool analogTransfer)
        {
            Provenance.SpeedUsed = speed;
            Provenance.SampleRate = "48K";
            Provenance.AnalogTransfer = analogTransfer;

            var result = Generator.Generate(Metadata, Provenance, model);
            var parts = result.Split(new[] { "\r\n" }, StringSplitOptions.None);

            // here, Dat files should default to stereo, unless it's a PRES-INT file. In which case,
            // should be mono
            var expectedSoundField = model.IsPreservationIntermediateVersion() ? "Mono" : "Stereo";

            var expectedTransfer = analogTransfer ? "ANALOGUE" : "DIGITAL";
            // build expected text
            var expected =
                $"A={expectedTransfer},M={expectedSoundField},T=Player manufacturer Player model;SNPlayer serial number;{expectedSpeedText}DAT,";

            Assert.That(parts[0], Is.EqualTo(expected));
        }

        [Test, TestCaseSource(nameof(_lines2And3Cases))]
        public void Line2ShouldBeCorrect(AbstractFile model, string sampleRate, string expectedSampleRate)
        {
            Provenance.SampleRate = sampleRate;
            
            var result = Generator.Generate(Metadata, Provenance, model);
            var parts = result.Split(new[] { "\r\n" }, StringSplitOptions.None);

            // here, Dat files should default to stereo, unless it's a PRES-INT file. In which case,
            // should be mono
            var expectedSoundField = model.IsPreservationIntermediateVersion() ? "Mono" : "Stereo";

            var expectedBitDepth = sampleRate == "96k" ? "24" : "16";
            // build texpected text
            var expected = $"A=PCM,F={expectedSampleRate},W={expectedBitDepth},M={expectedSoundField},T=Ad manufacturer Ad model;SNAd serial number;A/D,";
            Assert.That(parts[1], Is.EqualTo(expected));
        }

        [Test, TestCaseSource(nameof(_lines2And3Cases))]
        public void Line3ShouldBeCorrect(AbstractFile model, string sampleRate, string expectedSampleRate)
        {
            Provenance.SampleRate = sampleRate;
            var result = Generator.Generate(Metadata, Provenance, model);
            var parts = result.Split(new[] { "\r\n" }, StringSplitOptions.None);

            var expectedBitDepth = sampleRate == "96k" ? "24" : "16";
            // here, Dat files should default to stereo, unless it's a PRES-INT file. In which case,
            // should be mono
            var expectedSoundField = model.IsPreservationIntermediateVersion() ? "Mono" : "Stereo";

            // build texpected text
            var expected = $"A=PCM,F={expectedSampleRate},W={expectedBitDepth},M={expectedSoundField},T=Lynx AES16;DIO";
            Assert.That(parts[2], Is.EqualTo(expected));
        }

        [TestCase("")]
        [TestCase(null)]
        [TestCase(" ")]
        public void WhenSampleRateNotSet(string sampleRate)
        {
            Provenance.SampleRate = sampleRate;
            var exception = Assert.Throws<EmbeddedMetadataException>(() => Generator.Generate(Metadata, Provenance, PresModel));
            Assert.That(exception.Message, Is.EqualTo("Sample is blank in POD!"));
        }

        [Test]

        public void WhenSampleRateIsNotDefined()
        {
            Provenance.SampleRate = "foo";
            var exception = Assert.Throws<EmbeddedMetadataException>(() => Generator.Generate(Metadata, Provenance, PresModel));
            Assert.That(exception.Message, Is.EqualTo($"Unknown sample rate {Provenance.SampleRate}"));
        }
    }
}

