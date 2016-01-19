using System.Collections.Specialized;
using NUnit.Framework;
using Packager.Factories;

namespace Packager.Test.Factories
{
    [TestFixture]
    public class SettingsFactoryTests
    {
        [TestCase("value 1", "value 1")]
        [TestCase("value 2", "value 2")]
        [TestCase("", "")]
        [TestCase(" ", "")]
        [TestCase("   ", "")]
        public void GetStringValueShouldReturnCorrectValue(string rawValue, string expected)
        {
            var dictionary = new NameValueCollection
            {
                {"key", rawValue}
            };

            var factory = new SettingsFactory();

            Assert.That(factory.GetStringValue(dictionary, "key"), Is.EqualTo(expected));
        }

        [TestCase("1", 1)]
        [TestCase("2", 2)]
        [TestCase("-1", -1)]
        public void GetIntValueShouldReturnCorrectValue(string rawValue, int expected)
        {
            var dictionary = new NameValueCollection
            {
                {"key", rawValue}
            };

            var factory = new SettingsFactory();

            Assert.That(factory.GetIntValue(dictionary, "key", -100), Is.EqualTo(expected));
        }

        [TestCase("")]
        [TestCase(" ")]
        [TestCase("invalid")]
        public void GetIntValueShouldReturnDefaultValueIfRawValueInvalid(string rawValue)
        {
            var dictionary = new NameValueCollection
            {
                {"key", rawValue}
            };

            var factory = new SettingsFactory();

            Assert.That(factory.GetIntValue(dictionary, "key", -100), Is.EqualTo(-100));
        }

        [TestCase("value1,value2,value3")]
        [TestCase("value1, value2, value3")]
        [TestCase("value1, value2, value3, ,")]
        public void GetStringValuesShouldReturnCorrectValue(string rawValue)
        {
            var expected = new[] {"value1", "value2", "value3"};
            var dictionary = new NameValueCollection
            {
                {"key", rawValue}
            };

            var factory = new SettingsFactory();
            Assert.That(factory.GetStringValues(dictionary, "key"), Is.EquivalentTo(expected));
        }
    }
}