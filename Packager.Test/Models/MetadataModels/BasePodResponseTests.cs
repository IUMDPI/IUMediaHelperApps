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

            // Make the factory return some data, so we can verify that fields get expected data
            

            // here we don't need to set values, as we're just testing that the factory
            // gets called correctly
            Element = new XElement("pod");

            Factory.ToBooleanValue(Element, "success").Returns(true);
            Factory.ToStringValue(Element, "message").Returns("test message");

            Instance = new BasePodResponse();
            Instance.ImportFromXml(Element, Factory);
        }

        private BasePodResponse Instance { get; set; }
        private IImportableFactory Factory { get; set; }
        private XElement Element { get; set; }

        [Test]
        public void ItShouldUseCorrectPathToResolveMessage()
        {
            Factory.Received().ToStringValue(Element, "message");
        }

        [Test]
        public void SuccessShouldHaveCorrectValue()
        {
            Assert.That(Instance.Success, Is.True);
        }

        [Test]
        public void MessageShouldHaveCorrectValue()
        {
            Assert.That(Instance.Message, Is.EqualTo("test message"));
        }

        [Test]
        public void ItShouldUseCorrectPathToResolveSuccess()
        {
            Factory.Received().ToBooleanValue(Element, "success");
        }
    }
}