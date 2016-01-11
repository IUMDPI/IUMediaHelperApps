using NUnit.Framework;
using Packager.Attributes;

namespace Packager.Test.Attributes
{
    [TestFixture]
    public class FromListConfigSettingAttributeTests
    {

        [Test]
        public void ConstructorShouldWorkCorrectly()
        {
            var attribute = new FromListConfigSettingAttribute("name value");
            Assert.That(attribute.Name, Is.EqualTo("name value"));

        }

        [Test]
        public void ConvertShouldWorkCorrectly()
        {
            var attribute = new FromListConfigSettingAttribute("name value");

            const string value = "value 1,value 2 , value 3,";
            var expected = new[] {"value 1", "value 2", "value 3"};
            Assert.That(attribute.Convert(value), Is.EqualTo(expected));
        }
    }
}