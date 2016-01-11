using NUnit.Framework;
using Packager.Attributes;

namespace Packager.Test.Attributes
{
    [TestFixture]
    public class FFMPEGArgumentAttributeTests
    {
        [Test]
        public void ConstructorShouldWorkCorrectly()
        {
            var attribute = new FFMPEGArgumentAttribute("value");
            Assert.That(attribute.Value, Is.EqualTo("value"));
        }
    }
}