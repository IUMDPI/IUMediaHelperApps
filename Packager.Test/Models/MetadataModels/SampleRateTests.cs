using System.Reflection;
using NUnit.Framework;
using Packager.Models.PodMetadataModels;
using DescriptionAttribute = System.ComponentModel.DescriptionAttribute;

namespace Packager.Test.Models.MetadataModels
{
    [TestFixture]
    public class SampleRateTests
    {
        /* [TestCase("SampleRate32K", "32k")]
        [TestCase("SampleRate441K", "44.1k")]
        [TestCase("SampleRate48K", "48k")]
        [TestCase("SampleRate96K", "96k")]
        public void MembersShouldHaveCorrectDescriptionAttributes(string property, string expected)
        {
            var attribute = typeof(SampleRate).GetProperty(property).GetCustomAttribute<DescriptionAttribute>();
            Assert.That(attribute, Is.Not.Null);
            Assert.That(attribute.Description, Is.EqualTo(expected));
        }

        [Test]
        public void ClassShouldHaveFourProperties()
        {
            Assert.That(typeof(SampleRate).GetProperties().Length, Is.EqualTo(4));
        }

        [Test]
        public void AllPropertiesShouldHaveDescriptionAttribute()
        {
            foreach (var property in typeof(SampleRate).GetProperties())
            {
                Assert.That(property.GetCustomAttribute<DescriptionAttribute>(), Is.Not.Null);
            }
        }
    }*/
    }
}