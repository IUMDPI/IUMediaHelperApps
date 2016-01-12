using NUnit.Framework;
using Packager.Models.FileModels;

namespace Packager.Test.Models.FileModels
{
    public class QualityControlFileModelTests
    {
        private const string Filename = "MDPI_4890764553278906_01_pres.mkv";

        private QualityControlFileModel Model { get; set; }

        [SetUp]
        public void BeforeEach()
        {
            Model = new QualityControlFileModel(new ObjectFileModel(Filename));
        }

        [Test]
        public void SequenceIndicatorShouldBeCorrect()
        {
            Assert.That(Model.SequenceIndicator, Is.EqualTo(1));
        }

        [Test]
        public void ExtensionShouldBeCorrect()
        {
            Assert.That(Model.Extension, Is.EqualTo(".qctools.xml.gz"));
        }

        [Test]
        public void FileUseShouldBeCorrect()
        {
            Assert.That(Model.FileUse, Is.EqualTo("pres"));
        }

        [Test]
        public void BarcodeShouldBeCorrect()
        {
            Assert.That(Model.BarCode, Is.EqualTo("4890764553278906"));
        }

        [Test]
        public void ProjectCodeShouldBeCorrect()
        {
            Assert.That(Model.ProjectCode, Is.EqualTo("MDPI"));
        }

        [Test]
        public void ToFileNameShouldBeCorrect()
        {
            Assert.That(Model.ToFileName(), Is.EqualTo("MDPI_4890764553278906_01_pres.qctools.xml.gz"));
        }

        [Test]
        public void ToIntermediateFileNameShouldBeCorrect()
        {
            Assert.That(Model.ToIntermediateFileName(), Is.EqualTo("MDPI_4890764553278906_01_pres.qctools.xml"));
        }
    }
}