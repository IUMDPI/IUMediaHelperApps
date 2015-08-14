using System.Linq;
using System.Reflection;
using System.Xml.Serialization;
using NUnit.Framework;
using Packager.Models.BextModels;

namespace Packager.Test.Models.BextModels
{
    [TestFixture]
    public class ConformancePointDocumentFileCoreTests
    {
        [Test]
        public void AllFieldsShouldHaveMatchingSpecifiedFields()
        {
            foreach (var property in typeof (ConformancePointDocumentFileCore).GetProperties().Where(p => p.GetCustomAttribute<XmlIgnoreAttribute>() == null))
            {
                var expectedSpecifiedName = string.Format("{0}Specified", property.Name);
                var specifiedProperty = typeof (ConformancePointDocumentFileCore).GetProperty(expectedSpecifiedName);
                Assert.That(specifiedProperty, Is.Not.Null, string.Format("{0} should have matching {1} property", property.Name, expectedSpecifiedName));
                Assert.That(specifiedProperty.GetCustomAttribute<XmlIgnoreAttribute>(), Is.Not.Null, string.Format("{0} should have xml ignore attribute", expectedSpecifiedName));
            }
        }

        [TestCase("test value", true, "has value set")]
        [TestCase("", false, "is empty string")]
        [TestCase(" ", false, "is whitespace string")]
        [TestCase(null, false, "is null")]
        public void SpecifiedPropertyFieldsShouldReturnCorrectValue(string value, bool expectedResult, string description)
        {
            foreach (var property in typeof (ConformancePointDocumentFileCore).GetProperties().Where(p => p.GetCustomAttribute<XmlIgnoreAttribute>() == null))
            {
                var expectedSpecifiedName = string.Format("{0}Specified", property.Name);
                var specifiedProperty = typeof (ConformancePointDocumentFileCore).GetProperty(expectedSpecifiedName);
                Assert.That(specifiedProperty, Is.Not.Null, string.Format("{0} should have matching {1} property", property.Name, expectedSpecifiedName));

                var instance = new ConformancePointDocumentFileCore();
                property.SetValue(instance, value);

                Assert.That(
                    specifiedProperty.GetValue(instance),
                    Is.EqualTo(expectedResult),
                    string.Format("{0} should be {1} when {2} {3}", specifiedProperty.Name, expectedResult, property.Name, description));

                property.SetValue(instance, null);
            }
        }
    }
}