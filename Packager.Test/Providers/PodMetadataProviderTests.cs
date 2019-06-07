using Common.Models;
using NSubstitute;
using NUnit.Framework;
using Packager.Models.FileModels;
using Packager.Models.PodMetadataModels;
using Packager.Observers;
using Packager.Providers;
using Packager.Validators;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Packager.Test.Providers
{
    [TestFixture]
    public class PodMetadataProviderTests
    {
        private PodMetadataProvider Provider { get; set; }
        private IRestClient Client { get; set; }
        private IObserverCollection Observers { get; set; }
        private IValidatorCollection Validators { get; set; }

        [SetUp]
        public void BeforeEach()
        {
            Client = Substitute.For<IRestClient>();
            Observers = Substitute.For<IObserverCollection>();
            Validators = Substitute.For<IValidatorCollection>();

            Provider = new PodMetadataProvider(Client, Observers, Validators);
        }

        private static readonly AbstractFile ZipMaster = new AudioPreservationFile(new UnknownFile("MDPI_11111111111111_01_pres.zip"));
        private static readonly AbstractFile WavMaster = new AudioPreservationFile(new UnknownFile("MDPI_11111111111111_01_pres.wav"));
        private static readonly AbstractFile VideoMaster = new VideoPreservationFile(new UnknownFile("MDPI_11111111111111_01_pres.mkv"));

        private static readonly object[] AdjustFormatCases =
        {
            new object [] { MediaFormats.LacquerDisc, ZipMaster, MediaFormats.LacquerDiscIrene, "It should convert format to LacquerDiscIrene when master is .zip file and format is LacquerDisc"},
            new object [] { MediaFormats.LacquerDisc, WavMaster, MediaFormats.LacquerDisc, "It should not convert format to LacquerDiscIrene when master is .wav file and format is LacquerDisc"},
            new object [] { MediaFormats.Cdr, ZipMaster, MediaFormats.Cdr, "It should not convert format to LacquerDiscIrene when master is .zip file and format is not LacquerDisc"},
            new object [] { MediaFormats.Cdr, WavMaster, MediaFormats.Cdr, "It should not convert format to LacquerDiscIrene when master is .wav file and format is not LacquerDisc"},
            new object [] { MediaFormats.EightMillimeterVideo, VideoMaster, MediaFormats.EightMillimeterVideo, "It should not convert format to LacquerDiscIrene when format is video format"}
        };

        [TestCaseSource(nameof(AdjustFormatCases))]
        public void ItShouldAdjustFormatToLacquerDiscIreneCorrectly(IMediaFormat format, AbstractFile master, IMediaFormat expected, string description)
        {
            var metadata = new AudioPodMetadata
            {
                Format = format
            };

            var models = new List<AbstractFile>
            {
                master
            };

            var result = Provider.AdjustMediaFormat(metadata, models);
            Assert.That(metadata.Format, Is.EqualTo(expected), description);
        }
    }
}
