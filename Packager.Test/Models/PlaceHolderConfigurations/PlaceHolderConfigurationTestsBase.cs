using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Packager.Models.FileModels;
using Packager.Models.PlaceHolderConfigurations;

namespace Packager.Test.Models.PlaceHolderConfigurations
{
    [TestFixture]
    public abstract class PlaceHolderConfigurationTestsBase
    {
        private const string ProjectCode = "MDPI";
        private const string Barcode = "4890764553278906";

        protected static T MakeFile<T>(int sequence) where T : AbstractFile
        {
            var template = new UnknownFile($"{ProjectCode}_{Barcode}_{sequence}");
            return Activator.CreateInstance(typeof(T), template) as T;
        }

        protected IPlaceHolderConfiguration Configuration { get; set; }
        protected List<AbstractFile> Complete { get; set; }
        protected List<AbstractFile> PartiallyComplete { get; set; }
        protected List<AbstractFile> RequiresPlaceHolders { get; set; }
        protected List<AbstractFile> ExpectedPlaceHolders { get; set; }

        [Test]
        public void ItShouldReturnEmptyListForCompleteSequence()
        {
            var result = Configuration.GetPlaceHoldersToAdd(Complete);
            Assert.That(result, Is.Empty);
        }

        [Test]
        public void ItShouldReturnEmptyListForPartiallyCompleteSequence()
        {
            var result = Configuration.GetPlaceHoldersToAdd(PartiallyComplete);
            Assert.That(result, Is.Empty);
        }

        [Test]
        public void ItShouldReturnCorrectResultIfPlaceHoldersNeeded()
        {
            var expectedFileNames = ExpectedPlaceHolders.Select(fm => fm.Filename);
            var resultFileNames = Configuration.GetPlaceHoldersToAdd(RequiresPlaceHolders).Select(f => f.Filename);
            Assert.That(resultFileNames, Is.EquivalentTo(expectedFileNames));
        }

        [Test]
        public void PlaceHoldersShouldHavePlaceHolderFlagSet()
        {
            var results = Configuration.GetPlaceHoldersToAdd(RequiresPlaceHolders);
            Assert.That(results.Any(f => !f.IsPlaceHolder), Is.False);
        }
    }
}
