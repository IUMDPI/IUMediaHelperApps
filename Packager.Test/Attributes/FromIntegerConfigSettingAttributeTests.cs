using System.Linq;
using NUnit.Framework;
using Packager.Attributes;

namespace Packager.Test.Attributes
{
    [TestFixture]
    public class FromIntegerConfigSettingAttributeTests
    {

        [Test]
        public void ConstructorShouldWorkCorrectly()
        {
            var attribute = new FromIntegerConfigSettingAttribute("name value");
            Assert.That(attribute.Name, Is.EqualTo("name value"));

        }

        [TestCase("1", 1)]
        [TestCase("2", 2)]
        public void ConvertShouldWorkCorrectly(string value, int expected)
        {
            var attribute = new FromIntegerConfigSettingAttribute("name value");
            Assert.That(attribute.Convert(value), Is.EqualTo(expected));
        }

        [Test]
        public void ShouldReturnNullAndAddIssueIfCannotConvertValue()
        {
            var attribute = new FromIntegerConfigSettingAttribute("name value");
            Assert.That(attribute.Convert("invalid"), Is.EqualTo(null));
            Assert.That(attribute.Issues.Count, Is.EqualTo(1));
            Assert.That(attribute.Issues.First(), Is.EqualTo("\"invalid\" is not a valid integer value"));
        }
    }
}