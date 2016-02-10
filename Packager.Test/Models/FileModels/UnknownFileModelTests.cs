using NUnit.Framework;
using Packager.Models.FileModels;

namespace Packager.Test.Models.FileModels
{
    [TestFixture]
    public class UnknownFileModelTests
    {
        private const string GoodFileName = "mdpi_4890764553278906.txt";

        [Test]
        public void IsValidShouldAlwaysReturnFalse()
        {
            var model = new UnknownFile(GoodFileName);
            Assert.That(model.IsValid(), Is.False);
        }
    }
}