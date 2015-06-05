using NUnit.Framework;
using Packager.Extensions;
using Packager.Models.FileModels;

namespace Packager.Test.Extensions
{
    [TestFixture]
    public class FileModelExtensionTests
    {
        private readonly ObjectFileModel _objectFileModel = new ObjectFileModel("test.txt");
        private readonly UnknownFileModel _unknownFileModel = new UnknownFileModel();
        private readonly XmlFileModel _xmlFileModel = new XmlFileModel();

      
        [Test]
        public void IsObjectModelExtensionShouldWorkCorrectly()
        {
            Assert.That(_objectFileModel.IsObjectModel(), Is.True);
            Assert.That(_xmlFileModel.IsObjectModel(), Is.False);
            Assert.That(_unknownFileModel.IsObjectModel(), Is.False);
        }

        [Test]
        public void IsXmlModelExtensionShouldWorkCorrectly()
        {
            Assert.That(_objectFileModel.IsXmlModel(), Is.False);
            Assert.That(_xmlFileModel.IsXmlModel(), Is.True);
            Assert.That(_unknownFileModel.IsXmlModel(), Is.False);
        }

        [Test]
        public void IsUnknownModelExtensionShouldWorkCorrectly()
        {
            Assert.That(_objectFileModel.IsUnknownModel(), Is.False);
            Assert.That(_xmlFileModel.IsUnknownModel(), Is.False);
            Assert.That(_unknownFileModel.IsUnknownModel(), Is.True);
        }
    }
}