using NUnit.Framework;
using Packager.Models.FileModels;

namespace Packager.Test.Models.FileModels
{
    [TestFixture]
    public class GeneralFileModelTests
    {
        private readonly ObjectFileModel _fileModel = new ObjectFileModel("test.wav");

        [TestCase(".wav", true)]
        [TestCase(".mp4", false)]
        public void HasExtensionShouldWorkCorrectly(string value, bool expected)
        {
            Assert.That(_fileModel.HasExtension(value), Is.EqualTo(expected));
        }

        [TestCase("lowercase_barcode_01_pres.wav", "LOWERCASE")]
        [TestCase("mixedCase_barcode_01_pres.wav", "MIXEDCASE")]
        [TestCase("UPPERCASE_barcode_01_pres.wav", "UPPERCASE")]
        public void ProjectCodeShouldAlwaysBeUpperCase(string value, string expected)
        {
            var model = new ObjectFileModel(value);
            Assert.That(model.ProjectCode, Is.EqualTo(expected.ToUpperInvariant()));
        }
    }
}