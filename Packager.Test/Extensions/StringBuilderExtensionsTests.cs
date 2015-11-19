using System.Text;
using NUnit.Framework;
using Packager.Extensions;

namespace Packager.Test.Extensions
{
    [TestFixture]
    public class StringBuilderExtensionsTests
    {
        [Test]
        public void HasContentShouldReturnTrueIfContentPresent()
        {
            var builder = new StringBuilder("content");
            Assert.That(builder.HasContent(), Is.True);
        }

        [Test]
        public void HasContentShouldReturnFalseIfContentPresent()
        {
            var builder = new StringBuilder();
            Assert.That(builder.HasContent(), Is.False);
        }
    }
}