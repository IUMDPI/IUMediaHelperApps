using NSubstitute;
using NUnit.Framework;
using Packager.Factories.FFMPEGArguments;
using Packager.Models.SettingsModels;

namespace Packager.Test.Factories.ArgumentsFactories
{
    [TestFixture]
    public class CdrFFMPEGArgumentsGeneratorTests
    {
        private IProgramSettings ProgramSettings { get; set; }
        private CdrFFMPEGArgumentsGenerator Generator { get; set; }

        [SetUp]
        public void BeforeEach()
        {
            ProgramSettings = Substitute.For<IProgramSettings>();
            ProgramSettings.FFMPEGAudioAccessArguments.Returns("audio access arguments");
            Generator = new CdrFFMPEGArgumentsGenerator(ProgramSettings);
        }

        [Test]
        public void NormalizingArgumentsShouldBeCorrect()
        {
            var expected = "-acodec copy -write_bext 1 -rf64 auto -map_metadata -1".Split(' ');
            Assert.That(Generator.GetNormalizingArguments(),Is.EquivalentTo(expected));
        }

        [Test]
        public void AccessArgumentsShouldBeCorrect()
        {
            var expected = "-c:a aac -b:a 192k -strict -2 -ar 44100 -map_metadata -1".Split(' ');
            Assert.That(Generator.GetAccessArguments(), Is.EquivalentTo(expected));
        }

        [Test]
        public void ProdArgumentsShouldBeCorrect()
        {
            var expected = "-acodec copy -write_bext 1 -rf64 auto -map_metadata -1".Split(' ');
            Assert.That(Generator.GetProdOrMezzArguments(),Is.EquivalentTo(expected));
        }

    }
}