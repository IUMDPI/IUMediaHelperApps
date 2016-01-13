using System.Xml.Linq;
using NSubstitute;
using NUnit.Framework;
using Packager.Factories;
using Packager.Models.PodMetadataModels;

namespace Packager.Test.Models.MetadataModels
{
    [TestFixture]
    public class BasePodResponseTests
    {
        [SetUp]
        public void BeforeEach()
        {
            // Fake the factory, as we're just interested in how it's being called
            Factory = Substitute.For<IImportableFactory>();

            // here we don't need to set values, as we're just testing that the factory
            // gets called correctly
            Element = new XElement("pod");
            
            var instance = new BasePodResponse();
            instance.ImportFromXml(Element, Factory);
        }

        private IImportableFactory Factory { get; set; }
        private XElement Element { get; set; }

        [Test]
        public void ItShouldUseCorrectPathToResolveMessage()
        {
            Factory.Received().ToStringValue(Element, "message");
        }

        [Test]
        public void ItShouldUseCorrectPathToResolveSuccess()
        {
            Factory.Received().ToBooleanValue(Element, "success");
        }
    }
}