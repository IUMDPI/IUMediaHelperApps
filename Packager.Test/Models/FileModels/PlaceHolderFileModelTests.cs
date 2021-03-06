﻿using NUnit.Framework;
using Packager.Models.FileModels;

namespace Packager.Test.Models.FileModels
{
    [TestFixture]
    public class PlaceHolderFileModelTests
    {
        private const string ProjectCode = "MDPI";
        private const string BarCode = "4890764553278906";
        private const string Extension = ".wav";
        private const int SequenceIndicator = 1;
        private PlaceHolderFile PlaceHolderModel { get; set; }
        private AudioPreservationFile DerivedModel { get; set; }

        [SetUp]
        public void BeforeEach()
        {
            PlaceHolderModel = new PlaceHolderFile(ProjectCode, BarCode, SequenceIndicator, Extension);
            DerivedModel = new AudioPreservationFile(PlaceHolderModel);
        }

        [Test]
        public void PlaceHolderFlagShouldBeSet()
        {
            Assert.That(PlaceHolderModel.IsPlaceHolder, Is.True);        
        }

        [Test]
        public void ProjectCodeShouldBeCorrect()
        {
            Assert.That(PlaceHolderModel.ProjectCode, Is.EqualTo(ProjectCode));
        }

        [Test]
        public void BarcodeShouldBeCorrect()
        {
            Assert.That(PlaceHolderModel.BarCode, Is.EqualTo(BarCode));
        }

        [Test]
        public void SequenceIndicatorShouldBeCorrect()
        {
            Assert.That(PlaceHolderModel.SequenceIndicator, Is.EqualTo(SequenceIndicator));
        }

        [Test]
        public void PlaceHolderFlagShouldBeSetOnDerivedModel()
        {
            Assert.That(DerivedModel.IsPlaceHolder, Is.True);
        }

        [Test]
        public void ExtensionShouldBeCorrect()
        {
            Assert.That(PlaceHolderModel.Extension, Is.EqualTo(Extension));
            Assert.That(DerivedModel.Extension, Is.EqualTo(Extension));
        }
    }
}
