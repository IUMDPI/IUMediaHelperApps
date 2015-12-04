using System.IO;
using System.Linq;
using NUnit.Framework;
using Packager.Utilities;
using Packager.Utilities.Hashing;

namespace Packager.Test.Utilities
{
    [TestFixture]
    public class HasherTests
    {
        [Test]
        public void OutputShouldBe32Chars()
        {
            var bytes = new byte[] { 24, 16, 8, 32 };
            var result = Hasher.Hash(new MemoryStream(bytes));
            Assert.That(result.Length, Is.EqualTo(32));
        }

        [Test]
        public void OutputShouldOnlyContainCorrectCharacters()
        {
            var bytes = new byte[] { 24, 16, 8, 32 };
            var result = Hasher.Hash(new MemoryStream(bytes));
            var acceptableChars = new[] { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', 'a', 'b', 'c', 'd', 'e', 'f' };
            foreach (var c in result.ToCharArray())
            {
                Assert.That(acceptableChars.Contains(c), Is.True);
            }
        }
    }
}