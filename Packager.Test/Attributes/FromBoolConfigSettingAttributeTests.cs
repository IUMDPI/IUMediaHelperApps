using System.Linq;
using NUnit.Framework;
using Packager.Attributes;

namespace Packager.Test.Attributes
{
    [TestFixture]
    public class FromBoolConfigSettingAttributeTests
    {

        [Test]
        public void ConstructorShouldWorkCorrectly()
        {
            var attribute = new FromBoolConfigSettingAttribute("name value");
            Assert.That(attribute.Name, Is.EqualTo("name value"));

        }

        [TestCase("true", true)]
        [TestCase("false", false)]
        public void ConvertShouldWorkCorrectly(string value, bool expected)
        {
            var attribute = new FromBoolConfigSettingAttribute("name value");
            Assert.That(attribute.Convert(value), Is.EqualTo(expected));
        }

        [Test]
        public void ShouldReturnNullAndAddIssueIfCannotConvertValue()
        {
            var attribute = new FromBoolConfigSettingAttribute("name value");
            Assert.That(attribute.Convert("invalid"), Is.EqualTo(null));
            Assert.That(attribute.Issues.Count, Is.EqualTo(1));
            Assert.That(attribute.Issues.First(), Is.EqualTo("\"invalid\" is not a valid boolean value"));
        }
    }
}