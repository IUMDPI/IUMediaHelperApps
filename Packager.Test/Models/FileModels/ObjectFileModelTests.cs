﻿using NUnit.Framework;
using Packager.Models.FileModels;

namespace Packager.Test.Models.FileModels
{
    [TestFixture]
    public class ObjectFileModelTests
    {
        public enum UseCase
        {
            Access,
            Prod,
            Mezz,
            Pres
        }

        private const string Barcode = "4890764553278906";
        private const string ProjectCode = "MDPI";
        private const int SequenceIndicator = 1;
        private const string Extension = ".wav";
        private const string GoodPreservationFileName = "MDPI_4890764553278906_01_pres.wav";
        private const string GoodProductionFileName = "MDPI_4890764553278906_01_prod.wav";
        private const string GoodMezzanineFileName = "MDPI_4890764553278906_01_mezz.wav";
        private const string GoodAccessFileName = "MDPI_4890764553278906_01_access.wav";
        
        private const string NoProjectCodeFileName = "4890764553278906_01_pres.wav";
        private const string NoBarcodeFileName = "MDPI_01_pres.wav";
        private const string NoSequenceIndicatorFileName = "MDPI_4890764553278906_pres.wav";
        private const string NoUseFileName = "MDPI_4890764553278906_01.wav";
        private const string NoExtensionFileName = "MDPI_4890764553278906_01_pres";
        private const string AnotherBadFileName = "badFormat.wav";
        private const string EmptyFileName = "";
        private const string NullFileName = "";
        private const string WhitespaceFileName = "    ";

        [TestCase(GoodAccessFileName, UseCase.Access)]
        [TestCase(GoodProductionFileName, UseCase.Prod)]
        [TestCase(GoodMezzanineFileName, UseCase.Mezz)]
        [TestCase(GoodPreservationFileName, UseCase.Pres)]
        public void ShouldParseFileNameCorrectly(string path, UseCase useCase)
        {
            var model = new ObjectFileModel(path);
            Assert.That(model.IsValid(), Is.True);

            Assert.That(model.BarCode, Is.EqualTo(Barcode));
            Assert.That(model.SequenceIndicator, Is.EqualTo(SequenceIndicator));
            Assert.That(model.Extension, Is.EqualTo(Extension));
            Assert.That(model.ProjectCode, Is.EqualTo(ProjectCode));

            switch (useCase)
            {
                case UseCase.Access:
                    Assert.That(model.IsAccessVersion());
                    break;
                case UseCase.Mezz:
                    Assert.That(model.IsMezzanineVersion());
                    break;
                case UseCase.Prod:
                    Assert.That(model.IsProductionVersion());
                    break;
                case UseCase.Pres:
                    Assert.That(model.IsPreservationVersion());
                    break;
            }
        }

        [TestCase(GoodAccessFileName, UseCase.Access)]
        [TestCase(GoodProductionFileName, UseCase.Prod)]
        [TestCase(GoodMezzanineFileName, UseCase.Mezz)]
        [TestCase(GoodPreservationFileName, UseCase.Pres)]
        public void IsValidShouldReturnTrueForGoodFileNames(string path, UseCase useCase)
        {
            var model = new ObjectFileModel(path);
            Assert.That(model.IsValid(), Is.True);
        }

        [TestCase(NoBarcodeFileName)]
        [TestCase(NoProjectCodeFileName)]
        [TestCase(NoSequenceIndicatorFileName)]
        [TestCase(NoUseFileName)]
        [TestCase(NoExtensionFileName)]
        [TestCase(AnotherBadFileName)]
        [TestCase(EmptyFileName)]
        [TestCase(NullFileName)]
        [TestCase(WhitespaceFileName)]
        public void IsValidShouldReturnFalseIfFileNameBad(string path)
        {
            var model = new ObjectFileModel(path);
            Assert.That(model.IsValid(), Is.False);
        }

        [Test]
        public void ToFileNameShouldWorkCorrectly()
        {
            var model = new ObjectFileModel(GoodProductionFileName);
            Assert.That(model.ToFileName(), Is.EqualTo(GoodProductionFileName));
        }

        [Test]
        public void ToProductionFileModelShouldWorkCorrectly()
        {
            var model = new ObjectFileModel(GoodPreservationFileName);
            var result = model.ToProductionFileModel();
            Assert.That(result.IsProductionVersion());
            Assert.That(result.Extension, Is.EqualTo(ObjectFileModel.ProductionExtension));
        }

        [Test]
        public void ToAudioAccessFileModelShouldWorkCorrectly()
        {
            var model = new ObjectFileModel(GoodPreservationFileName);
            var result = model.ToAudioAccessFileModel();
            Assert.That(result.IsAccessVersion());
            Assert.That(result.Extension, Is.EqualTo(ObjectFileModel.AudioAccessExtension));
        }

        [Test]
        public void ToVideoAccessFileModelShouldWorkCorrectly()
        {
            var model = new ObjectFileModel(GoodPreservationFileName);
            var result = model.ToVideoAccessFileModel();
            Assert.That(result.IsAccessVersion());
            Assert.That(result.Extension, Is.EqualTo(ObjectFileModel.VideoAccessExtension));
        }


        [Test]
        public void ToMezzanineFileModelShouldWorkCorrectly()
        {
            var model = new ObjectFileModel(GoodPreservationFileName);
            var result = model.ToMezzanineFileModel();
            Assert.That(result.IsMezzanineVersion());
            Assert.That(result.Extension, Is.EqualTo(ObjectFileModel.MezzanineExtension));
        }
    }
}