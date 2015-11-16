using System.Reflection;
using NUnit.Framework;
using Packager.Models.PodMetadataModels;
using DescriptionAttribute = System.ComponentModel.DescriptionAttribute;

namespace Packager.Test.Models.MetadataModels
{
    [TestFixture]
    public class SoundFieldTests
    {
      /*  [TestCase("Mono", "Mono")]
        [TestCase("Stereo", "Stereo")]
        [TestCase("UnknownSoundField", "Unknown")]
        public void MembersShouldHaveCorrectDescriptionAttributes(string property, string expected)
        {
            var attribute = typeof(SoundField).GetProperty(property).GetCustomAttribute<DescriptionAttribute>();
            Assert.That(attribute, Is.Not.Null);
            Assert.That(attribute.Description, Is.EqualTo(expected));
        }

        [Test]
        public void ClassShouldHaveThreeProperties()
        {
            Assert.That(typeof(SoundField).GetProperties().Length, Is.EqualTo(3));
        }

        [Test]
        public void AllPropertiesShouldHaveDescriptionAttribute()
        {
            foreach (var property in typeof(SoundField).GetProperties())
            {
                Assert.That(property.GetCustomAttribute<DescriptionAttribute>(), Is.Not.Null);
            }
        }*/
    }
}