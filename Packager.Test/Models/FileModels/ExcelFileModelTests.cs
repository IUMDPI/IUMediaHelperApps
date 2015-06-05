using NUnit.Framework;
using Packager.Models.FileModels;

namespace Packager.Test.Models.FileModels
{
    [TestFixture]
    public class ExcelFileModelTests
    {
        private const string Barcode = "4890764553278906";
        private const string ProjectCode = "MDPI";
        private const string Extension = ".xlsx";
        private const string GoodFileName = "MDPI_4890764553278906.xlsx";
        private const string NoProjectCodeFileName = "4890764553278906.xlsx";
        private const string NoBarcodeFileName = "mdpi.xlsx";
        private const string NoExtensionFileName = "MDPI_4890764553278906";
        private const string AnotherBadFileName = "badFormat.xlsx";
        private const string EmptyFileName = "";
        private const string NullFileName = "";
        private const string WhitespaceFileName = "    ";

        [Test]
        public void IsValidShouldReturnOkForValidFilename()
        {
            var model = new ExcelFileModel(GoodFileName);
            Assert.That(model.IsValid());
        }

        [Test]
        public void ShouldParseFileNameCorrectly()
        {
            var model = new ExcelFileModel(GoodFileName);
            Assert.That(model.ProjectCode, Is.EqualTo(ProjectCode));
            Assert.That(model.BarCode, Is.EqualTo(Barcode));
            Assert.That(model.Extension, Is.EqualTo(Extension));
        }

        [Test]
        public void ToFileNameShouldWorkCorrectly()
        {
            var model = new ExcelFileModel(GoodFileName);
            Assert.That(model.ToFileName(), Is.EqualTo(GoodFileName));
        }

        [TestCase(NoExtensionFileName)]
        [TestCase(NoProjectCodeFileName)]
        [TestCase(NoBarcodeFileName)]
        [TestCase(AnotherBadFileName)]
        [TestCase(EmptyFileName)]
        [TestCase(NullFileName)]
        [TestCase(WhitespaceFileName)]
        public void IsValidShouldReturnFalseForInvalidFileName(string filename)
        {
            var model = new ExcelFileModel(filename);
            Assert.That(model.IsValid(), Is.False);
        }
    }
}