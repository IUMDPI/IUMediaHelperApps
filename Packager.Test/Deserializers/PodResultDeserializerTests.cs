using System.Xml.Linq;
using NSubstitute;
using NUnit.Framework;
using Packager.Deserializers;
using Packager.Factories;
using Packager.Models.PodMetadataModels;
using RestSharp;

namespace Packager.Test.Deserializers
{
    public class PodResultDeserializerTests
    {
        [Test]
        public void IfResponseContentPresentShouldCallFactoryCorrectly()
        {
            var response = Substitute.For<IRestResponse>();
            response.Content.Returns(
                "<?xml version=\"1.0\" encoding=\"UTF-8\"?>" +
                "<pod xmlns:xsi = \"http://www.w3.org/2001/XMLSchema-instance\">" +
                "<success>true</success>" +
                "</pod>");

            var factory = Substitute.For<IImportableFactory>();
            factory.ToImportable<AudioPodMetadata>(Arg.Any<XElement>()).Returns(new AudioPodMetadata());

            var deserializer = new PodResultDeserializer(factory);

            var result = deserializer.Deserialize<AudioPodMetadata>(response);

            Assert.That(result, Is.Not.Null);
            factory.Received().ToImportable<AudioPodMetadata>(Arg.Any<XElement>());
        }

        [Test]
        public void IfNoResponseContentShouldReturnNull()
        {
            var response = Substitute.For<IRestResponse>();
            var factory = Substitute.For<IImportableFactory>();
            factory.ToImportable<AudioPodMetadata>(Arg.Any<XElement>()).Returns(new AudioPodMetadata());

            var deserializer = new PodResultDeserializer(factory);

            var result = deserializer.Deserialize<AudioPodMetadata>(response);

            Assert.That(result, Is.Null);
            factory.DidNotReceive().ToImportable<AudioPodMetadata>(Arg.Any<XElement>());
        }
    }
}