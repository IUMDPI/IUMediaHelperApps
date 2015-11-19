using System.Collections.Generic;
using NUnit.Framework;
using Packager.Extensions;
using Packager.Models.FileModels;
using Packager.Models.PodMetadataModels;

namespace Packager.Test.Extensions
{
    [TestFixture]
    public class DigitalFileExtensionTests
    {
        [Test]
        public void GetFileProvenanceNormalizesExtensionAndReturnsCorrectDigitalFileForModel()
        {
            const string fileName1WithoutExtension = "test_1111_01_pres";
            const string fileName2WithoutExtension = "test_1111_01_prod";
            const string fileName1 = fileName1WithoutExtension + ".wav";
            const string fileName2 = fileName2WithoutExtension + ".wav";

            var model1 = new ObjectFileModel(fileName1);
            var model2 = new ObjectFileModel(fileName2);

            var provenances = new List<DigitalAudioFile>
            {
                new DigitalAudioFile {Filename = fileName1WithoutExtension},
                new DigitalAudioFile {Filename = fileName2WithoutExtension}
            };

            Assert.That(provenances.GetFileProvenance(model1).Filename, Is.EqualTo(fileName1WithoutExtension));
            Assert.That(provenances.GetFileProvenance(model2).Filename, Is.EqualTo(fileName2WithoutExtension));
        }

        [Test]
        public void GetFileProvenanceReturnsCorrectDigitalFileForModel()
        {
            const string fileName1 = "test_1111_01_pres.wav";
            const string fileName2 = "test_1111_01_prod.wav";

            var model1 = new ObjectFileModel(fileName1);
            var model2 = new ObjectFileModel(fileName2);

            var provenances = new List<DigitalAudioFile>
            {
                new DigitalAudioFile {Filename = fileName1},
                new DigitalAudioFile {Filename = fileName2}
            };

            Assert.That(provenances.GetFileProvenance(model1).Filename, Is.EqualTo(fileName1));
            Assert.That(provenances.GetFileProvenance(model2).Filename, Is.EqualTo(fileName2));
        }

        [Test]
        public void GetFileProvenanceShouldReturnNullIfNotFoundAndDefaultValueNotSpecified()
        {
            const string fileName1 = "test_1111_01_pres.wav";
            const string fileName2 = "test_1111_01_prod.wav";

            var model2 = new ObjectFileModel(fileName2);

            var provenances = new List<DigitalAudioFile>
            {
                new DigitalAudioFile {Filename = fileName1}
            };

            Assert.That(provenances.GetFileProvenance(model2), Is.Null);
        }
    }
}