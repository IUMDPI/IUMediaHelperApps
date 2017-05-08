using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common.Models;
using NUnit.Framework;

namespace Common.Tests.Models
{
    [TestFixture]
    public class MediaFormatsTests
    {
        private static object[] _formatsCorrectCases = {
            new object[] {MediaFormats.AudioCassette, "audiocassette", "Audio Cassette"},
            new object[] {MediaFormats.OpenReelAudioTape,"open reel audio tape", "Open Reel Audio Tape"},
            new object[] {MediaFormats.Lp, "lp", "Lp"},
            new object[] {MediaFormats.Cdr, "cd-r", "CD Rom"},
            new object[] {MediaFormats.FortyFive, "45", "45"},
            new object[] {MediaFormats.LacquerDisc, "lacquer disc", "Lacquer Disc"},
            new object[] {MediaFormats.Cylinder,"cylinder", "Cylinder"},
            new object[] {MediaFormats.SeventyEight, "78", "78"},
            new object[] {MediaFormats.Vhs, "vhs", "VHS"},
            new object[] {MediaFormats.Betacam, "betacam", "Betacam"},
            new object[] {MediaFormats.BetacamAnamorphic, "betacam:Anamorphic", "Betacam: Anamorphic"},
            new object[] {MediaFormats.Dat,"dat", "DAT"},
            new object[] {MediaFormats.OneInchOpenReelVideoTape,"1-inch open reel video tape", "1-inch Open Reel Video Tape"},
            new object[] {MediaFormats.EightMillimeterVideo,"8mm video", "8mm Video"},
            new object[] {MediaFormats.EightMillimeterVideoQuadaudio, "8mm video:quadaudio", "8mm Video: Quadaudio"},
            new object[] {MediaFormats.Umatic,"u-matic", "U-matic"},
            new object[] {MediaFormats.Betamax, "betamax", "Betamax"}
        };

        private static object[] _getFormatCases =
        {
            new object[] {"audiocassette",MediaFormats.AudioCassette},
            new object[] {"open reel audio tape", MediaFormats.OpenReelAudioTape},
            new object[] {"lp", MediaFormats.Lp},
            new object[] {"cd-r",MediaFormats.Cdr},
            new object[] {"45",MediaFormats.FortyFive},
            new object[] {"lacquer disc",MediaFormats.LacquerDisc},
            new object[] {"cylinder",MediaFormats.Cylinder},
            new object[] {"78",MediaFormats.SeventyEight},
            new object[] {"vhs",MediaFormats.Vhs},
            new object[] {"betacam",MediaFormats.Betacam},
            new object[] {"betacam:Anamorphic",MediaFormats.BetacamAnamorphic},
            new object[] {"dat",MediaFormats.Dat},
            new object[] {"1-inch open reel video tape",MediaFormats.OneInchOpenReelVideoTape},
            new object[] {"8mm video",MediaFormats.EightMillimeterVideo},
            new object[] {"8mm video:quadaudio",MediaFormats.EightMillimeterVideoQuadaudio},
            new object[] {"u-matic",MediaFormats.Umatic},
            new object[] {"betamax",MediaFormats.Betamax},
            new object[] {string.Empty, new UnknownMediaFormat(string.Empty)},
            new object[] {null, new UnknownMediaFormat(null) },
            new object[] {"some other value", new UnknownMediaFormat("some other value") }
        };

        [Test, TestCaseSource(nameof(_formatsCorrectCases))]
        public void FormatsShouldBeCorrect(IMediaFormat format, string expectedKey, string expectedProperName)
        {
            Assert.That(format.Key, Is.EqualTo(expectedKey), $"Key should be {expectedKey}");
            Assert.That(format.ProperName, Is.EqualTo(expectedProperName), $"Proper name should be {expectedProperName}");
        }

        [Test, TestCaseSource(nameof(_getFormatCases))]
        public void GetFormatShouldBeCorrect(string key, IMediaFormat expected)
        {
            var result = MediaFormats.GetFormat(key);
            var description = $"{key} should produce {expected.ProperName} media-format object";

            if (expected is UnknownMediaFormat)
            {
                Assert.That(result is UnknownMediaFormat, description);
                Assert.That(result.ProperName, Is.EqualTo(key));
            }
            else
            {
                Assert.That(result, Is.EqualTo(expected), description);
            }
        }
    }
}
