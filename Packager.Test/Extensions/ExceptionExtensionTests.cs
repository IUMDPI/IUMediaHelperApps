using System;
using NUnit.Framework;
using Packager.Extensions;

namespace Packager.Test.Extensions
{
    [TestFixture]
    public class ExceptionExtensionTests
    {
        [Test]
        public void GetFirstMessageShouldReturnCorrectResult()
        {
            var firstException = new Exception("first message");
            var secondException = new Exception("second message", firstException);
            var thirdException = new Exception("third message", secondException);

            Assert.That(firstException.GetBaseMessage(), Is.EqualTo("first message"));
            Assert.That(secondException.GetBaseMessage(), Is.EqualTo("first message"));
            Assert.That(thirdException.GetBaseMessage(), Is.EqualTo("first message"));
        }
    }
}