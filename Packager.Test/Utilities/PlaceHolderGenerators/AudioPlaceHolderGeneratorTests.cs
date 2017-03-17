using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Packager.Models.FileModels;
using Packager.Utilities.PlaceHolderGenerators;

namespace Packager.Test.Utilities.PlaceHolderGenerators
{
    [TestFixture]
    public class AudioPlaceHolderGeneratorTests
    {
        private const string ProjectCode = "MDPI";
        private const string Barcode = "4890764553278906";
        
        private AudioPreservationFile Sequence1PresFile { get; set; }
        private ProductionFile Sequence1ProdFile { get; set; }
        private AccessFile Sequence2AccessFile { get; set; }
        private AudioPreservationFile Sequence2PresFile { get; set; }
        private ProductionFile Sequence2ProdFile { get; set; }
        private AccessFile Sequence3AccessFile { get; set; }
        private AudioPreservationFile Sequence3PresFile { get; set; }
        private ProductionFile Sequence3ProdFile { get; set; }
        private AccessFile Sequence1AccessFile { get; set; }
        private TiffImageFile Sequence1TiffFile { get; set; }
        private TiffImageFile Sequence2TiffFile { get; set; }
        private TiffImageFile Sequence3TiffFile { get; set; }

        [SetUp]
        public void BeforeEach()
        {
            Sequence1PresFile = new AudioPreservationFile(new UnknownFile($"{ProjectCode}_{Barcode}_1"));
            Sequence1ProdFile = new ProductionFile(new UnknownFile($"{ProjectCode}_{Barcode}_1"));
            Sequence1AccessFile = new AccessFile(new UnknownFile($"{ProjectCode}_{Barcode}_1"));
            Sequence2PresFile = new AudioPreservationFile(new UnknownFile($"{ProjectCode}_{Barcode}_2"));
            Sequence2ProdFile = new ProductionFile(new UnknownFile($"{ProjectCode}_{Barcode}_2"));
            Sequence2AccessFile = new AccessFile(new UnknownFile($"{ProjectCode}_{Barcode}_2"));
            Sequence3PresFile = new AudioPreservationFile(new UnknownFile($"{ProjectCode}_{Barcode}_3"));
            Sequence3ProdFile = new ProductionFile(new UnknownFile($"{ProjectCode}_{Barcode}_3"));
            Sequence3AccessFile = new AccessFile(new UnknownFile($"{ProjectCode}_{Barcode}_3"));
            Sequence1TiffFile = new TiffImageFile(new UnknownFile($"{ProjectCode}_{Barcode}_1"));
            Sequence2TiffFile = new TiffImageFile(new UnknownFile($"{ProjectCode}_{Barcode}_2"));
            Sequence3TiffFile = new TiffImageFile(new UnknownFile($"{ProjectCode}_{Barcode}_3"));
        }

        [Test]
        public void ItShouldReturnEmptyListIfNoPlaceHoldersRequired()
        {
            var placeHolderGenerator = new AudioPlaceHolderGenerator();

            var completeSequence = new List<AbstractFile>
            {
                Sequence1PresFile, Sequence1ProdFile, Sequence1AccessFile,  Sequence1TiffFile
            };
            var result = placeHolderGenerator.GetPlaceHoldersToAdd(completeSequence);
            Assert.That(result, Is.Empty);

        }

        [Test]
        public void ItShouldReturnEmptyIfSequenceIsPartiallyComplete()
        {
            var partiallyCompleteSequence = new List<AbstractFile>
            {
                Sequence1PresFile, Sequence1ProdFile, Sequence1AccessFile,  Sequence1TiffFile, // complete sequence
                Sequence2PresFile, Sequence2ProdFile, Sequence2TiffFile, // partially complete
                Sequence3TiffFile // fully incomplete sequences
            };

            var expectedFileNames = new List<AbstractFile>
            {
                Sequence3PresFile, Sequence3ProdFile,Sequence3AccessFile
            }
            .Select(f => f.Filename);

            var placeHolderGenerator = new AudioPlaceHolderGenerator();
            var resultFileNames = placeHolderGenerator.GetPlaceHoldersToAdd(partiallyCompleteSequence).Select(f => f.Filename);
            Assert.That(resultFileNames, Is.EquivalentTo(expectedFileNames));
        }

        [Test]
        public void IfPlaceHoldersNeededShouldReturnCorrectList()
        {
            var incompleteSequence =new List<AbstractFile>
            {
                Sequence1PresFile, Sequence1ProdFile, Sequence1AccessFile,  Sequence1TiffFile, // complete sequence
                Sequence2TiffFile, Sequence3TiffFile // add 2 fully incomplete sequences
            };

            var expectedFileNames = new List<AbstractFile>
            {
                Sequence2PresFile, Sequence2ProdFile, Sequence2AccessFile,
                Sequence3PresFile, Sequence3ProdFile,Sequence3AccessFile,
            }
            .Select(f=>f.Filename);

            var placeHolderGenerator = new AudioPlaceHolderGenerator();
            var resultFileNames = placeHolderGenerator.GetPlaceHoldersToAdd(incompleteSequence).Select(f=>f.Filename);
            Assert.That(resultFileNames, Is.EquivalentTo(expectedFileNames));
        }

        [Test]
        public void PlaceHoldersShouldHavePlaceHolderFlagSet()
        {
            var incompleteSequence = new List<AbstractFile>
            {
                Sequence1PresFile, Sequence1ProdFile, Sequence1AccessFile,  Sequence1TiffFile, // complete sequence
                Sequence2TiffFile, Sequence3TiffFile // add 2 fully incomplete sequences
            };
            var placeHolderGenerator = new AudioPlaceHolderGenerator();
            var results = placeHolderGenerator.GetPlaceHoldersToAdd(incompleteSequence);
            Assert.That(results.Any(f=>!f.PlaceHolder), Is.False);
        }

    }
}
