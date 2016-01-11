using NUnit.Framework;
using Packager.Attributes;

namespace Packager.Test.Attributes
{
    [TestFixture]
    public class FromStringConfigSettingAttributeTests
    {
       
        [Test] 
        public void ConstructorShouldWorkCorrectly()
        {
            var attribute = new FromStringConfigSettingAttribute("name value");
            Assert.That(attribute.Name, Is.EqualTo("name value"));
           
        }

        [Test]
        public void ConvertShouldWorkCorrectly()
        {
            var attribute = new FromStringConfigSettingAttribute("name value");
            Assert.That(attribute.Convert("test"), Is.EqualTo("test"));
        }
    }
}