using NSubstitute;
using NUnit.Framework;
using Packager.Factories.FFMPEGArguments;
using Packager.Models.SettingsModels;

namespace Packager.Test.Factories.ArgumentsFactories
{
    [TestFixture]
    public class VideoFFMPEGArgumentsGeneratorTests
    {
        private IProgramSettings ProgramSettings { get; set; }
        private VideoFFMPEGArgumentsGenerator Generator { get; set; }

        [SetUp]
        public void BeforeEach()
        {
            ProgramSettings = Substitute.For<IProgramSettings>();
            ProgramSettings.FFMPEGVideoAccessArguments.Returns("video access arguments");
            ProgramSettings.FFMPEGVideoMezzanineArguments.Returns("video mezzanine arguments");

            Generator = new VideoFFMPEGArgumentsGenerator(ProgramSettings);
        }

        [Test]
        public void NormalizingArgumentsShouldBeCorrect()
        {
            var expected = "-map 0 -acodec copy -vcodec copy".Split(' ');
            Assert.That(Generator.GetNormalizingArguments(), Is.EquivalentTo(expected));
        }

        [Test]
        public void MezzanineArgumentsShouldBeCorrect()
        {
            Assert.That(Generator.GetProdOrMezzArguments(), 
                Is.EquivalentTo(ProgramSettings.FFMPEGVideoMezzanineArguments.Split(' ')));
        }

        [Test]
        public void AccessArgumentsShouldBeCorrect()
        {
            Assert.That(Generator.GetAccessArguments(), 
                Is.EquivalentTo(ProgramSettings.FFMPEGVideoAccessArguments.Split(' ')));
        }
    }
}