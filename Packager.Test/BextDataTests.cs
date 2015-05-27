using System.Linq;
using NUnit.Framework;
using Packager.Extensions;
using Packager.Models;

namespace Packager.Test
{
    [TestFixture]
    public class BextDataTests
    {
        [Test]
        public void ShouldGenerateCorrectCommandLineArgsArray()
        {
            const string expectedDescription =
                "Indiana University, Bloomington. William and Gayle Cook Music Library. TP-S .A1828 81-4-17 v. 1. File use:";

            const string expectedIARL =
                "Indiana University, Bloomington. William and Gayle Cook Music Library. TP-S .A1828 81-4-17 v. 1. File use:";

            const string expectedICMT = "Indiana University, Bloomington. William and Gayle Cook Music Library.";

            var data = new BextData
            {
                Description = expectedDescription,
                IARL = expectedIARL,
                ICMT = expectedICMT
            };
            var result = data.GenerateCommandArgs();
            Assert.That(result.Count(), Is.EqualTo(3));
            Assert.That(result[0].Contains(expectedDescription.ToQuoted()));
            Assert.That(result[1].Contains(expectedIARL.ToQuoted()));
            Assert.That(result[2].Contains(expectedICMT.ToQuoted()));
        }
    }
}