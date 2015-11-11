using System.Reflection;
using NUnit.Framework;
using Packager.Models.PodMetadataModels;
using DescriptionAttribute = System.ComponentModel.DescriptionAttribute;

namespace Packager.Test.Models.MetadataModels
{
    [TestFixture]
    public class PlaybackSpeedTests
    {
        [TestCase("ZeroPoint9375Ips", ".9375 ips")]
        [TestCase("OnePoint875Ips", "1.875 ips")]
        [TestCase("ThreePoint75Ips", "3.75 ips")]
        [TestCase("SevenPoint5Ips", "7.5 ips")]
        [TestCase("FifteenIps", "15 ips")]
        [TestCase("ThirtyIps", "30 ips")]
        [TestCase("UnknownPlaybackSpeed", "Unknown")]
        public void MembersShouldHaveCorrectDescriptionAttributes(string property, string expected)
        {
            var attribute = typeof(PlaybackSpeed).GetProperty(property).GetCustomAttribute<DescriptionAttribute>();
            Assert.That(attribute, Is.Not.Null);
            Assert.That(attribute.Description, Is.EqualTo(expected));
        }

        [Test]
        public void ClassShouldHaveSevenProperties()
        {
            Assert.That(typeof(PlaybackSpeed).GetProperties().Length, Is.EqualTo(7));
        }

        [Test]
        public void AllPropertiesShouldHaveDescriptionAttribute()
        {
            foreach (var property in typeof(PlaybackSpeed).GetProperties())
            {
                Assert.That(property.GetCustomAttribute<DescriptionAttribute>(), Is.Not.Null);
            }
        }
    }
}