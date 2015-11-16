using System.Reflection;
using NUnit.Framework;
using Packager.Models.PodMetadataModels;
using DescriptionAttribute = System.ComponentModel.DescriptionAttribute;

namespace Packager.Test.Models.MetadataModels
{
    [TestFixture]
    public class TrackConfigurationTests
    {
       /* [TestCase("FullTrack", "Full track")]
        [TestCase("HalfTrack", "Half track")]
        [TestCase("QuarterTrack", "Quarter track")]
        [TestCase("UnknownTrack", "Unknown")]
        public void MembersShouldHaveCorrectDescriptionAttributes(string property, string expected)
        {
            var attribute = typeof (TrackConfiguration).GetProperty(property).GetCustomAttribute<DescriptionAttribute>();
            Assert.That(attribute, Is.Not.Null);
            Assert.That(attribute.Description, Is.EqualTo(expected));
        }

        [Test]
        public void ClassShouldHaveFourProperties()
        {
            Assert.That(typeof(TrackConfiguration).GetProperties().Length, Is.EqualTo(4));
        }

        [Test]
        public void AllPropertiesShouldHaveDescriptionAttribute()
        {
            foreach (var property in typeof (TrackConfiguration).GetProperties())
            {
                Assert.That(property.GetCustomAttribute< DescriptionAttribute>(), Is.Not.Null);
            }
        }*/
    }
}