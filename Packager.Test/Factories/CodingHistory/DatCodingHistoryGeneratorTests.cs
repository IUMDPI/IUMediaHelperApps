using Common.Models;
using NUnit.Framework;
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
            new object[] {PresModel },
            new object[] {PresIntModel },
            new object[] {ProdModel },
        };

        private static readonly object[] _line1Cases =
        {
            new object[] {PresModel, "8.5 ips", "8.5 ips;" },
            new object[] {PresModel, "8.5 ips, 1.1 ips, 2.1 ips", "8.5 ips;1.1 ips;2.1 ips;" },
            new object[] {PresModel, null, "" },
            new object[] {PresModel, "", "" },
           
            new object[] {PresIntModel, "8.5 ips", "8.5 ips;"  },
            new object[] {PresIntModel, "8.5 ips, 1.1 ips, 2.1 ips", "8.5 ips;1.1 ips;2.1 ips;" },
            new object[] {PresIntModel, null, "" },
            new object[] {PresIntModel, "", "" },
            
            new object[] {ProdModel, "8.5 ips", "8.5 ips;" },
            new object[] {ProdModel, "8.5 ips, 1.1 ips, 2.1 ips", "8.5 ips;1.1 ips;2.1 ips;" },
            new object[] {ProdModel, null, "" },
            new object[] {ProdModel, "", "" },
        };

        [Test, TestCaseSource(nameof(_line1Cases))]
        public void Line1ShouldBeCorrect(AbstractFile model, string speed, string expectedSpeedText)
        {
            Provenance.SpeedUsed = speed;

            var result = Generator.Generate(Metadata, Provenance, model);
            var parts = result.Split(new[] { "\r\n" }, StringSplitOptions.None);

            // here, Dat files should default to stereo, unless it's a PRES-INT file. In which case,
            // should be mono
            var expectedSoundField = model.IsPreservationIntermediateVersion() ? "Mono" : "Stereo";
            
            // build expected text
            var expected =
                $"A=DIGITAL,M={expectedSoundField},T=Player manufacturer Player model;SNPlayer serial number;{expectedSpeedText}DAT,";

            Assert.That(parts[0], Is.EqualTo(expected));
        }

        [Test, TestCaseSource(nameof(_lines2And3Cases))]
        public void Line2ShouldBeCorrect(AbstractFile model)
        {
            var result = Generator.Generate(Metadata, Provenance, model);
            var parts = result.Split(new[] { "\r\n" }, StringSplitOptions.None);

            // here, Dat files should default to stereo, unless it's a PRES-INT file. In which case,
            // should be mono
            var expectedSoundField = model.IsPreservationIntermediateVersion() ? "Mono" : "Stereo";

            // build texpected text
            var expected = $"A=PCM,F=96000,W=24,M={expectedSoundField},T=Ad manufacturer Ad model;SNAd serial number;A/D,";
            Assert.That(parts[1], Is.EqualTo(expected));
        }

        [Test, TestCaseSource(nameof(_lines2And3Cases))]
        public void Line3ShouldBeCorrect(AbstractFile model)
        {
            var result = Generator.Generate(Metadata, Provenance, model);
            var parts = result.Split(new[] { "\r\n" }, StringSplitOptions.None);

            // here, Dat files should default to stereo, unless it's a PRES-INT file. In which case,
            // should be mono
            var expectedSoundField = model.IsPreservationIntermediateVersion() ? "Mono" : "Stereo";

            // build texpected text
            var expected = $"A=PCM,F=96000,W=24,M={expectedSoundField},T=Lynx AES16;DIO";
            Assert.That(parts[2], Is.EqualTo(expected));
        }
    }
}

