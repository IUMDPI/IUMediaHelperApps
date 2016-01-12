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
        [TestCase("true", true)]
        [TestCase("True", true)]
        [TestCase("false", false)]
        [TestCase("False", false)]
        public void ImportShouldSetSuccessCorrectly(string value, bool expected)
        {
            var element = new XElement("pod", new XElement("success") {Value = value});

            var instance = new BasePodResponse();
            instance.ImportFromXml(element, Substitute.For<IImportableFactory>());

            Assert.That(instance.Success, Is.EqualTo(expected));
        }

        [TestCase("message 1")]
        [TestCase("message 2")]
        public void ImportShouldSetMessageCorrectly(string value)
        {
            var element = new XElement("pod", new XElement("message") { Value = value });

            var instance = new BasePodResponse();
            instance.ImportFromXml(element, Substitute.For<IImportableFactory>());

            Assert.That(instance.Message, Is.EqualTo(value));
        }
    }
}