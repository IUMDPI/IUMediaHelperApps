using System.Reflection;
using NUnit.Framework;
using Packager.Attributes;
using Packager.Models.BextModels;
using Packager.Utilities;

namespace Packager.Test.Models.BextModels
{
    [TestFixture]
    public class BextMetadataTests
    {
        [Test]
        public void AllFieldsShouldHaveMatchingBextFieldAttributes()
        {
            foreach (var property in typeof (BextMetadata).GetProperties())
            {
                var attribute = property.GetCustomAttribute<BextFieldAttribute>();
                Assert.That(attribute, Is.Not.Null, $"property {property.Name} should have BextFieldAttribute");

                if (attribute.Field.Equals(BextFields.History))
                {
                    Assert.That(property.Name, Is.EqualTo("CodingHistory"), "History field should be named CodingHistory");
                }
                else
                {
                    Assert.That(property.Name, Is.EqualTo(attribute.Field.ToString()), $"{property.Name} field should be named {attribute.Field}");
                }
            }
        }
    }
}