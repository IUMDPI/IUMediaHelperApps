using System.Collections.Generic;
using NUnit.Framework;
using Packager.Models.FileModels;

namespace Packager.Test.Models.FileModels
{
    [TestFixture]
    public class FileModelFactoryTests
    {
        private const string WavExtension = ".wav";
        private const string Mp4Extension = ".mp4";
        private const string ExcelFileName = "mdpi_4890764553278906.xlsx";
        private const string WavFileName = "mdpi_4890764553278906_01_pres.wav";
        private const string Mp4FileName = "mdpi_5890764553278906_01_pres.mp4";
        private const string UnknownFilename = "unknown.txt";
        private FileModelFactory _factory;

        [SetUp]
        public void BeforeEach()
        {
            _factory = new FileModelFactory(new List<string> {WavExtension, Mp4Extension});
        }

        [TestCase(WavFileName)]
        [TestCase(Mp4FileName)]
        public void ShouldGenerateCorrectModelForArtifactFiles(string filename)
        {
            var result = _factory.GetModel(filename);
            Assert.That(result as ArtifactFileModel, Is.Not.Null);
        }

        [Test]
        public void ShouldGenerateCorrectModelForExcelFiles()
        {
            var result = _factory.GetModel(ExcelFileName);
            Assert.That(result as ExcelFileModel, Is.Not.Null);
        }

        [Test]
        public void ShouldAssignUnknownFileModelToOtherFileExtensions()
        {
            var result = _factory.GetModel(UnknownFilename);
            Assert.That(result as UnknownFileModel, Is.Not.Null);
        }
    }
}