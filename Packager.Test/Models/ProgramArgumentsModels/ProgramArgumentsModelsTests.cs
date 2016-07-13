using NUnit.Framework;
using Packager.Models.ProgramArgumentsModels;

namespace Packager.Test.Models.ProgramArgumentsModels
{
    [TestFixture]
    public class ProgramArgumentsModelsTests
    {
        [TestCase("-noninteractive", false)]
        [TestCase("-NonInteractive", false)]
        [TestCase("-NONInteractive", false)]
        [TestCase("", true)]
        [TestCase("-blah", true)]
        public void ItShouldSetInteractiveCorrectly(string argument, bool expectedValue)
        {
            var arguments = new ProgramArguments(new []{argument});
            Assert.That(arguments.Interactive, Is.EqualTo(expectedValue));
        }

        [Test]
        public void ItShouldSetInteractiveToTrueIfNoArgumentsPresent()
        {
            var arguments = new ProgramArguments(new string[0]);
            Assert.That(arguments.Interactive, Is.True);
        }
    }
}
