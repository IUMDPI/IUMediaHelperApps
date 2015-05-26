using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NUnit.Framework;
using Packager.Models;
using Assert = NUnit.Framework.Assert;

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
            
            var data = new BextData { Description = expectedDescription,
                IARL = expectedIARL, ICMT = expectedICMT };
            var result = data.GenerateCommandArgs();
            Assert.That(result.Count(), Is.EqualTo(3));
            Assert.That(result[0].Contains(expectedDescription));
            Assert.That(result[1].Contains(expectedIARL));
            Assert.That(result[2].Contains(expectedICMT));
        }

    }
}
