using System.Reflection;
using NUnit.Framework;
using Packager.Models.PodMetadataModels;
using DescriptionAttribute = System.ComponentModel.DescriptionAttribute;

namespace Packager.Test.Models.MetadataModels
{
    [TestFixture]
    public class TapeThicknessTests
    {
        [TestCase("ZeroPoint5Mils", ".5")]
        [TestCase("OneMils", "1.0")]
        [TestCase("OnePoint5Mils", "1.5")]
        public void MembersShouldHaveCorrectDescriptionAttributes(string property, string expected)
        {
            var attribute = typeof(TapeThickness).GetProperty(property).GetCustomAttribute<DescriptionAttribute>();
            Assert.That(attribute, Is.Not.Null);
            Assert.That(attribute.Description, Is.EqualTo(expected));
        }

        [Test]
        public void ClassShouldHaveThreeProperties()
        {
            Assert.That(typeof(TapeThickness).GetProperties().Length, Is.EqualTo(3));
        }

        [Test]
        public void AllPropertiesShouldHaveDescriptionAttribute()
        {
            foreach (var property in typeof(TapeThickness).GetProperties())
            {
                Assert.That(property.GetCustomAttribute<DescriptionAttribute>(), Is.Not.Null);
            }
        }
    }
}