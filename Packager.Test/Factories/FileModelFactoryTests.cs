using Common.Models;
using NUnit.Framework;
using Packager.Factories;
using Packager.Models.FileModels;

namespace Packager.Test.Factories
{
    [TestFixture]
    public class FileModelFactoryTests
    {
        private const string UnknownFilename = "unknown.txt";

        [TestCase("mdpi_4890764553278906_01_pres.wav")]
        [TestCase("mdpi_4890764553278906_01_Pres.Wav")]
        public void ShouldGenerateAudioPreservationModelsCorrectly(string filename)
        {
            var result = FileModelFactory.GetModel(filename) as AudioPreservationFile;
            Assert.That(result, Is.Not.Null);
            Assert.That(result.ProjectCode, Is.EqualTo("MDPI"));
            Assert.That(result.BarCode, Is.EqualTo("4890764553278906"));
            Assert.That(result.SequenceIndicator, Is.EqualTo(1));
            Assert.That(result.FileUsage, Is.EqualTo(FileUsages.PreservationMaster));
            Assert.That(result.Extension, Is.EqualTo(".wav"));
        }

        [TestCase("mdpi_4890764553278906_01_presInt.wav")]
        [TestCase("mdpi_4890764553278906_01_presint.Wav")]
        public void ShouldGenerateAudioPreservationIntermediateModelsCorrectly(string filename)
        {
            var result = FileModelFactory.GetModel(filename) as AudioPreservationIntermediateFile;
            Assert.That(result, Is.Not.Null);
            Assert.That(result.ProjectCode, Is.EqualTo("MDPI"));
            Assert.That(result.BarCode, Is.EqualTo("4890764553278906"));
            Assert.That(result.SequenceIndicator, Is.EqualTo(1));
            Assert.That(result.FileUsage, Is.EqualTo(FileUsages.PreservationIntermediateMaster));
            Assert.That(result.Extension, Is.EqualTo(".wav"));
        }

        [TestCase("mdpi_4890764553278906_01_pres.mkv")]
        [TestCase("mdpi_4890764553278906_01_Pres.mkv")]
        public void ShouldGenerateVideoPreservationModelsCorrectly(string filename)
        {
            var result = FileModelFactory.GetModel(filename) as VideoPreservationFile;
            Assert.That(result, Is.Not.Null);
            Assert.That(result.ProjectCode, Is.EqualTo("MDPI"));
            Assert.That(result.BarCode, Is.EqualTo("4890764553278906"));
            Assert.That(result.SequenceIndicator, Is.EqualTo(1));
            Assert.That(result.FileUsage, Is.EqualTo(FileUsages.PreservationMaster));
            Assert.That(result.Extension, Is.EqualTo(".mkv"));
        }

        [TestCase("mdpi_4890764553278906_01_presInt.mkv")]
        [TestCase("mdpi_4890764553278906_01_presint.mkv")]
        public void ShouldGenerateVideoPreservationIntermediateModelsCorrectly(string filename)
        {
            var result = FileModelFactory.GetModel(filename) as VideoPreservationIntermediateFile;
            Assert.That(result, Is.Not.Null);
            Assert.That(result.ProjectCode, Is.EqualTo("MDPI"));
            Assert.That(result.BarCode, Is.EqualTo("4890764553278906"));
            Assert.That(result.SequenceIndicator, Is.EqualTo(1));
            Assert.That(result.FileUsage, Is.EqualTo(FileUsages.PreservationIntermediateMaster));
            Assert.That(result.Extension, Is.EqualTo(".mkv"));
        }

        [TestCase("mdpi_4890764553278906_01_mezz.mov")]
        [TestCase("mdpi_4890764553278906_01_Mezz.Mov")]
        public void ShouldGenerateVideoMezzanineModelsCorrectly(string filename)
        {
            var result = FileModelFactory.GetModel(filename) as MezzanineFile;
            Assert.That(result, Is.Not.Null);
            Assert.That(result.ProjectCode, Is.EqualTo("MDPI"));
            Assert.That(result.BarCode, Is.EqualTo("4890764553278906"));
            Assert.That(result.SequenceIndicator, Is.EqualTo(1));
            Assert.That(result.FileUsage, Is.EqualTo(FileUsages.MezzanineFile));
            Assert.That(result.Extension, Is.EqualTo(".mov"));
        }

        [TestCase("mdpi_4890764553278906_01_prod.wav")]
        [TestCase("mdpi_4890764553278906_01_Prod.Wav")]
        public void ShouldGenerateAudioProductionModelsCorrectly(string filename)
        {
            var result = FileModelFactory.GetModel(filename) as ProductionFile;
            Assert.That(result, Is.Not.Null);
            Assert.That(result.ProjectCode, Is.EqualTo("MDPI"));
            Assert.That(result.BarCode, Is.EqualTo("4890764553278906"));
            Assert.That(result.SequenceIndicator, Is.EqualTo(1));
            Assert.That(result.FileUsage, Is.EqualTo(FileUsages.ProductionMaster));
            Assert.That(result.Extension, Is.EqualTo(".wav"));
        }

        [TestCase("mdpi_4890764553278906_01_access.mp4")]
        [TestCase("mdpi_4890764553278906_01_Access.Mp4")]
        public void ShouldGenerateAccessModelsCorrectly(string filename)
        {
            var result = FileModelFactory.GetModel(filename) as AccessFile;
            Assert.That(result, Is.Not.Null);
            Assert.That(result.ProjectCode, Is.EqualTo("MDPI"));
            Assert.That(result.BarCode, Is.EqualTo("4890764553278906"));
            Assert.That(result.SequenceIndicator, Is.EqualTo(1));
            Assert.That(result.FileUsage, Is.EqualTo(FileUsages.AccessFile));
            Assert.That(result.Extension, Is.EqualTo(".mp4"));
        }

        [Test]
        public void ShouldAssignRawObectFileModelToOtherFileExtensions()
        {
            var result = FileModelFactory.GetModel(UnknownFilename);
            Assert.That(result as UnknownFile, Is.Not.Null);
        }
    }
}