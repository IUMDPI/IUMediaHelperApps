using NSubstitute;
using NUnit.Framework;
using Packager.Factories.FFMPEGArguments;
using Packager.Models.SettingsModels;

namespace Packager.Test.Factories.ArgumentsFactories
{
    [TestFixture]
    public class AudioFFMPEGArgumentsGeneratorTests
    {
        private IProgramSettings ProgramSettings { get; set; }

        [SetUp]
        public void BeforeEach()
        {
            ProgramSettings = Substitute.For<IProgramSettings>();
            ProgramSettings.FFMPEGAudioAccessArguments.Returns("audio access arguments");
        }

        [Test]
        public void NormalizingArgumentsShouldBeCorrect()
        {
            var generator = new AudioFFMPEGArgumentsGenerator(ProgramSettings);
            var expected = "-acodec copy -write_bext 1 -rf64 auto -map_metadata -1".Split(' ');
            Assert.That(generator.GetNormalizingArguments(),Is.EquivalentTo(expected));
        }

        [Test]
        public void AccessArgumentsShouldBeCorrect()
        {
            var generator = new AudioFFMPEGArgumentsGenerator(ProgramSettings);
            var expected = "audio access arguments -map_metadata -1".Split(' ');
            Assert.That(generator.GetAccessArguments(), Is.EquivalentTo(expected));
        }

        [TestCase("-test", "-test -write_bext 1 -rf64 auto")]
        [TestCase("-test -write_bext 1", "-test -write_bext 1 -rf64 auto")]
        [TestCase("-write_bext 1 -test", "-write_bext 1 -test -rf64 auto")]
        [TestCase("-test -write_bext 1 -rf64 auto", "-test -write_bext 1 -rf64 auto")]
        [TestCase("-test -rf64 auto -write_bext 1", "-test -rf64 auto -write_bext 1")]
        public void ProdArgumentsShouldBeCorrect(string args, string expected)
        {
            ProgramSettings.FFMPEGAudioProductionArguments.Returns(args);
            var generator = new AudioFFMPEGArgumentsGenerator(ProgramSettings);
            Assert.That(generator.GetProdOrMezzArguments(), Is.EquivalentTo(expected.Split(' ')));
        }

        

    }
}
