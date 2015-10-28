using System.Text;
using NUnit.Framework;
using Packager.Kludges;

namespace Packager.Test.Kludges
{
    [TestFixture]
    public class CodingHistoryKludgeTests
    {
        [Test]
        public void ItShouldAddASpaceIfLengthEven()
        {
            var builder = new StringBuilder("even");
            CodingHistoryKludge.KludgeForFFMpeg(builder);
            Assert.That(builder.Length, Is.EqualTo(5));
            Assert.That(builder.Length %2, Is.Not.EqualTo(0));
            Assert.That(builder.ToString().EndsWith(" "));
        }

        [Test]
        public void ItShouldNotAddASpaceIfLengthOdd()
        {
            var builder = new StringBuilder("odd");
            CodingHistoryKludge.KludgeForFFMpeg(builder);
            Assert.That(builder.Length, Is.EqualTo(3));
            Assert.That(builder.Length % 2, Is.Not.EqualTo(0));
        }


    }
}