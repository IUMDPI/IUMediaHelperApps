using NUnit.Framework;
using Packager.Models.EmbeddedMetadataModels;
using Packager.Utilities.ProcessRunners;

namespace Packager.Test.Models.EmbeddedMetadataTests.EmbeddedVideoMetadataTests
{
    [TestFixture]
    public abstract class AbstractEmbeddedVideoMetadataTests
    {
        private AbstractEmbeddedVideoMetadata Metadata { get; set; }
        private ArgumentBuilder Arguments { get; set; }

        public class EmbeddedVideoPreservationMetadataTests : AbstractEmbeddedVideoMetadataTests
        {
            [SetUp]
            public void BeforeEach()
            {
                Metadata = new EmbeddedVideoPreservationMetadata
                {
                    Comment = "comment",
                    Description = "description",
                    MasteredDate = "1/1/2016",
                    Title = "title"
                };

                Arguments = Metadata.AsArguments();
            }

            [Test]
            public void ArgumentsShouldContainDateDigitized()
            {
                Assert.That(Arguments.Contains("-metadata date_digitized=\"1/1/2016\""));
            }
        }

        public class EmbeddedVideoMezzanineMetadataTests : AbstractEmbeddedVideoMetadataTests
        {
            [SetUp]
            public void BeforeEach()
            {
                Metadata = new EmbeddedVideoMezzanineMetadata
                {
                    Comment = "comment",
                    Description = "description",
                    MasteredDate = "1/1/2016",
                    Title = "title"
                };

                Arguments = Metadata.AsArguments();
            }

            [Test]
            public void ArgumentsShouldContainDateDigitized()
            {
                Assert.That(Arguments.Contains("-metadata date=\"1/1/2016\""));
            }
        }

        [Test]
        public void ArgumentsShouldContainComment()
        {
            Assert.That(Arguments.Contains("-metadata comment=\"comment\""));
        }

        [Test]
        public void ArgumentsShouldContainDescription()
        {
            Assert.That(Arguments.Contains("-metadata description=\"description\""));
        }

        [Test]
        public void ArgumentsShouldContainTitle()
        {
            Assert.That(Arguments.Contains("-metadata title=\"title\""));
        }
    }
}