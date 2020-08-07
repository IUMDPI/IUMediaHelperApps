using System;
using Common.Models;
using NUnit.Framework;
using Packager.Extensions;
using Packager.Factories.CodingHistory;
using Packager.Models.FileModels;
using Packager.Models.PodMetadataModels;

namespace Packager.Test.Factories.CodingHistory
{
    public class LacquerOrCylinderCodingHistoryGeneratorTests : CodingHistoryGeneratorTestsBase
    {

        private static AudioPreservationToneReferenceFile PresRefModel
            => BaseModel.ConvertTo<AudioPreservationToneReferenceFile>();
        private static AudioPreservationIntermediateToneReferenceFile IntRefModel
            => BaseModel.ConvertTo<AudioPreservationIntermediateToneReferenceFile>();

        private static readonly object[] _line1Cases =
        {
            new object[] {PresModel, "8.5 ips", "8.5 ips;"},
            new object[] {PresModel, "8.5 ips, 1.1 ips, 2.1 ips", "8.5 ips;1.1 ips;2.1 ips;"},
            new object[] {PresModel, null, ""},
            new object[] {PresModel, "", ""},
            new object[] {PresIntModel, "8.5 ips", "8.5 ips;"},
            new object[] {PresIntModel, "8.5 ips, 1.1 ips, 2.1 ips", "8.5 ips;1.1 ips;2.1 ips;"},
            new object[] {PresIntModel, null, ""},
            new object[] {PresIntModel, "", ""},
            new object[] {PresRefModel, "8.5 ips", "8.5 ips;"},
            new object[] {PresRefModel, "8.5 ips, 1.1 ips, 2.1 ips", "8.5 ips;1.1 ips;2.1 ips;"},
            new object[] {PresRefModel, null, ""},
            new object[] {PresRefModel, "", ""},
            new object[] {IntRefModel, "8.5 ips", "8.5 ips;"},
            new object[] {IntRefModel, "8.5 ips, 1.1 ips, 2.1 ips", "8.5 ips;1.1 ips;2.1 ips;"},
            new object[] {IntRefModel, null, ""},
            new object[] {IntRefModel, "", ""},
            new object[] {ProdModel, "8.5 ips", "8.5 ips;"},
            new object[] {ProdModel, "8.5 ips, 1.1 ips, 2.1 ips", "8.5 ips;1.1 ips;2.1 ips;"},
            new object[] {ProdModel, null, ""},
            new object[] {ProdModel, "", ""},
        };

        private static readonly object[] _lines2And3Cases =
        {
            new object[] {PresModel, "Stereo" },
            new object[] {PresIntModel, "Mono" },
            new object[] {ProdModel,  "Mono" },
            new object[] {PresRefModel, "Mono"},
            new object[] {IntRefModel, "Mono"}
        };

        [SetUp]
        public override void BeforeEach()
        {
            base.BeforeEach();

            Metadata = new AudioPodMetadata
            {
                Format = MediaFormats.Cylinder
            };

            Generator = new LacquerOrCylinderCodingHistoryGenerator();
        }

        [Test, TestCaseSource(nameof(_line1Cases))]
        public void Line1ShouldBeCorrect(AbstractFile model, string speed, string expectedSpeedText)
        {
            Provenance.SpeedUsed = speed;

            var result = Generator.Generate(Metadata, Provenance, model);
            var parts = result.Split(new[] { "\r\n" }, StringSplitOptions.None);

            var expected =
                $"A=ANALOGUE,M=Mono,T=Player manufacturer Player model;SNPlayer serial number;{expectedSpeedText}Cylinder,";

            Assert.That(parts[0], Is.EqualTo(expected));
        }

        [Test, TestCaseSource(nameof(_lines2And3Cases))]
        public void Line2ShouldBeCorrect(AbstractFile model, string soundField)
        {
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

            var expected = $"A=PCM,F=96000,W=24,M={soundField},T=Lynx AES16;DIO";
            Assert.That(parts[2], Is.EqualTo(expected));
        }
    }
}