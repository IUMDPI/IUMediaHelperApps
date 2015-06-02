using NUnit.Framework;
using Packager.Extensions;
using Packager.Models.FileModels;

namespace Packager.Test.Extensions
{
    [TestFixture]
    public class FileModelExtensionTests
    {
        private readonly ArtifactFileModel _artifactFileModel = new ArtifactFileModel("test.txt");
        private readonly ExcelFileModel _excelFileModel = new ExcelFileModel();
        private readonly UnknownFileModel _unknownFileModel = new UnknownFileModel();
        private readonly XmlFileModel _xmlFileModel = new XmlFileModel();

        [Test]
        public void IsExcelModelExtensionShouldWorkCorrectly()
        {
            Assert.That(_excelFileModel.IsExcelModel(), Is.True);
            Assert.That(_artifactFileModel.IsExcelModel(), Is.False);
            Assert.That(_xmlFileModel.IsExcelModel(), Is.False);
            Assert.That(_unknownFileModel.IsExcelModel(), Is.False);
        }

        [Test]
        public void IsArtifactModelExtensionShouldWorkCorrectly()
        {
            Assert.That(_excelFileModel.IsArtifactModel(), Is.False);
            Assert.That(_artifactFileModel.IsArtifactModel(), Is.True);
            Assert.That(_xmlFileModel.IsArtifactModel(), Is.False);
            Assert.That(_unknownFileModel.IsArtifactModel(), Is.False);
        }

        [Test]
        public void IsXmlModelExtensionShouldWorkCorrectly()
        {
            Assert.That(_excelFileModel.IsXmlModel(), Is.False);
            Assert.That(_artifactFileModel.IsXmlModel(), Is.False);
            Assert.That(_xmlFileModel.IsXmlModel(), Is.True);
            Assert.That(_unknownFileModel.IsXmlModel(), Is.False);
        }

        [Test]
        public void IsUnknownModelExtensionShouldWorkCorrectly()
        {
            Assert.That(_excelFileModel.IsUnknownModel(), Is.False);
            Assert.That(_artifactFileModel.IsUnknownModel(), Is.False);
            Assert.That(_xmlFileModel.IsUnknownModel(), Is.False);
            Assert.That(_unknownFileModel.IsUnknownModel(), Is.True);
        }
    }
}