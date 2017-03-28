using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NSubstitute;
using NUnit.Framework.Internal;
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
        private Dictionary<string, IPlaceHolderConfiguration> ConfigurationDictionary { get; set; }
        private PlaceHolderFactory Factory { get; set; }
        private IObserverCollection Observers { get; set; }
        private List<AbstractFile> FileList { get; set; }

        [SetUp]
        public void BeforeEach()
        {
            Configuration = Substitute.For<IPlaceHolderConfiguration>();
            Configuration.GetPlaceHoldersToAdd(null).ReturnsForAnyArgs(new List<AbstractFile>());
            ConfigurationDictionary = new Dictionary<string, IPlaceHolderConfiguration>
            {
                {"format",  Configuration }
            };
            Observers = Substitute.For<IObserverCollection>();
            Factory = new PlaceHolderFactory(ConfigurationDictionary, Observers);
        }

        [TestCase("format")]
        [TestCase("Format")]
        [TestCase("FORMAT")]
        public void FactoryShouldUseConfigurationIfDefinedForFormat(string format)
        {
            Factory.GetPlaceHoldersToAdd(format, FileList);
            Configuration.Received().GetPlaceHoldersToAdd(FileList);
        }

        [Test]
        public void FactoryShouldReturnEmptyListIfFormatNotDefined()
        {
            var result = Factory.GetPlaceHoldersToAdd("undefined format", FileList);
            Assert.That(result, Is.Empty);
        }

        [Test]
        public void FactoryShouldLogIfFormatUnknown()
        {
            Factory.GetPlaceHoldersToAdd("undefined format", FileList);
            Observers.Received().Log("No place-holder configuration found for format {0}", "undefined format");
        }
    }
}
