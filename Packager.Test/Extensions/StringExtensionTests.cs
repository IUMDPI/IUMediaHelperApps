using NUnit.Framework;
using Packager.Extensions;

namespace Packager.Test.Extensions
{
    [TestFixture]
    public class StringExtensionTests
    {
        [TestCase(@"testing """, @"testing \""")]
        public void NormalizeForCommandLineShouldEscapeQuotesCorrectly(string original, string expected)
        {
            var result = original.NormalizeForCommandLine();
            Assert.That(result, Is.EqualTo(expected));
        }
    }
}