using System.Collections.Generic;
using Common.Models;
using NSubstitute;
using NUnit.Framework;
using Packager.Factories;
using Packager.Models.FileModels;
using Packager.Models.PlaceHolderConfigurations;
using Packager.Observers;

namespace Packager.Test.Factories
{
    [TestFixture]
    public class PlaceFolderFactoryTests
    {
        private IPlaceHolderConfiguration Configuration { get; set; }
        private Dictionary<IMediaFormat, IPlaceHolderConfiguration> ConfigurationDictionary { get; set; }
        private PlaceHolderFactory Factory { get; set; }
        private IObserverCollection Observers { get; set; }
        private List<AbstractFile> FileList { get; set; }

        [SetUp]
        public void BeforeEach()
        {
            Configuration = Substitute.For<IPlaceHolderConfiguration>();
            Configuration.GetPlaceHoldersToAdd(null).ReturnsForAnyArgs(new List<AbstractFile>());
            ConfigurationDictionary = new Dictionary<IMediaFormat, IPlaceHolderConfiguration>
            {
                {MediaFormats.Betacam,  Configuration }
            };
            Observers = Substitute.For<IObserverCollection>();
            Factory = new PlaceHolderFactory(ConfigurationDictionary, Observers);
        }

        [Test]
        public void FactoryShouldUseConfigurationIfDefinedForFormat()
        {
            Factory.GetPlaceHoldersToAdd(MediaFormats.Betacam, FileList);
            Configuration.Received().GetPlaceHoldersToAdd(FileList);
        }

        [Test]
        public void FactoryShouldReturnEmptyListIfFormatNotDefined()
        {
            var result = Factory.GetPlaceHoldersToAdd(new UnknownMediaFormat("unknown format"), FileList);
            Assert.That(result, Is.Empty);
        }

        [Test]
        public void FactoryShouldLogIfFormatUnknown()
        {
            var unknownFormat = new UnknownMediaFormat("unknown format");
            Factory.GetPlaceHoldersToAdd(unknownFormat, FileList);
            Observers.Received().Log("No placeholder configuration found for format {0}", unknownFormat.ProperName);
        }
    }
}
