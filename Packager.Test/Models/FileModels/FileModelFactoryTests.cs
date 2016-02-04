using NUnit.Framework;
using Packager.Factories;
using Packager.Models.FileModels;

namespace Packager.Test.Models.FileModels
{
    [TestFixture]
    public class FileModelFactoryTests
    {
        private const string WavFileName = "mdpi_4890764553278906_01_pres.wav";
        private const string Mp4FileName = "mdpi_5890764553278906_01_pres.mkv";
        private const string UnknownFilename = "unknown.txt";

        [TestCase(WavFileName)]
        [TestCase(Mp4FileName)]
        public void ShouldGenerateCorrectModelForObjectFiles(string filename)
        {
            var result = FileModelFactory.GetModel(filename);
            Assert.That(result, Is.Not.Null);
        }

        [Test]
        public void ShouldAssignRawObectFileModelToOtherFileExtensions()
        {
            var result = FileModelFactory.GetModel(UnknownFilename);
            Assert.That(result as UnknownFile, Is.Not.Null);
        }
    }
}