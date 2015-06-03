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
    }
}