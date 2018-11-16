using Common.Models;
using NUnit.Framework;
using Packager.Models.FileModels;

namespace Packager.Test.Models.FileModels
{
    [TestFixture]
    public class XmlFileModelTests
    {
        [TestCase("mdpi", "4890764553278906", true)]
        [TestCase("", "4890764553278906", false)]
        [TestCase(" ", "4890764553278906", false)]
        [TestCase("mdpi", "", false)]
        [TestCase("mdpi", " ", false)]
        [TestCase("", "", false)]
        [TestCase(" ", " ", false)]
        public void IsValidShouldReturnTrueIfBarCodeAndProjectCodeOk(string projectCode, string barCode, bool expected)
        {
            var model = new XmlFile(projectCode, barCode);
            Assert.That(model.IsImportable(), Is.EqualTo(expected));
        }

        [TestCase("mdpi", "4890764553278906", "MDPI_4890764553278906.xml")]
        public void ToFileNameShouldWorkCorrectly(string projectCode, string barCode, string expected)
        {
            var model = new XmlFile(projectCode, barCode);
            Assert.That(model.Filename, Is.EqualTo(expected));
        }
    }

    [TestFixture]
    public class TextFileModelTests
    {
        [TestCase("mdpi", "4890764553278906", true)]
        [TestCase("", "4890764553278906", false)]
        [TestCase(" ", "4890764553278906", false)]
        [TestCase("mdpi", "", false)]
        [TestCase("mdpi", " ", false)]
        [TestCase("", "", false)]
        [TestCase(" ", " ", false)]
        public void IsValidShouldReturnTrueIfBarCodeAndProjectCodeOk(string projectCode, string barCode, bool expected)
        {
            var model = new TextFile(new UnknownFile($"{projectCode}_{barCode}.text"));
            Assert.That(model.IsImportable(), Is.EqualTo(expected));
        }

        [TestCase("mdpi", "4890764553278906", "MDPI_4890764553278906.txt")]
        public void ToFileNameShouldWorkCorrectly(string projectCode, string barCode, string expected)
        {
            var model = new TextFile(new UnknownFile($"{projectCode}_{barCode}.text"));
            Assert.That(model.Filename, Is.EqualTo(expected));
        }

        [Test]
        public void SequenceIndicatorShouldAlwaysEqualOne()
        {
            var model = new TextFile(new UnknownFile("MDPI_4890764553278906.txt"));
            Assert.That(model.SequenceIndicator, Is.EqualTo(1));
        }

         [Test]
        public void FileUsageShouldBeCorrect()
        {
            var model = new TextFile(new UnknownFile("MDPI_4890764553278906.txt"));
            Assert.That(model.FileUsage, Is.EqualTo(FileUsages.TextFile));
        }
    }
}