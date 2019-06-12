using Common.Models;
using NSubstitute;
using NUnit.Framework;
using Packager.Exceptions;
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

        private const string ZipMasterSide1FileName = "MDPI_11111111111111_01_pres.zip";
        private const string WavMasterSide1FileName = "MDPI_11111111111111_01_pres.wav";
        private const string PresIntSide1Filename = "MDPI_11111111111111_01_presInt.wav";

        private const string ZipMasterSide2FileName = "MDPI_11111111111111_02_pres.zip";
        private const string WavMasterSide2FileName = "MDPI_11111111111111_02_pres.wav";
        private const string PresIntSide2Filename = "MDPI_11111111111111_02_presInt.wav";

        private const string MkvMasterFileName = "MDPI_11111111111111_01_pres.mkv";

        private static readonly AbstractFile Side1ZipMaster = new AudioPreservationFile(new UnknownFile(ZipMasterSide1FileName));
        private static readonly AbstractFile Side1WavMaster = new AudioPreservationFile(new UnknownFile(WavMasterSide1FileName));
        private static readonly AbstractFile Side1PresIntMaster = new AudioPreservationIntermediateFile(new UnknownFile(PresIntSide1Filename));

        private static readonly AbstractFile Side2ZipMaster = new AudioPreservationFile(new UnknownFile(ZipMasterSide2FileName));
        private static readonly AbstractFile Side2WavMaster = new AudioPreservationFile(new UnknownFile(WavMasterSide2FileName));
        private static readonly AbstractFile Side2PresIntMaster = new AudioPreservationIntermediateFile(new UnknownFile(PresIntSide2Filename));

        private static readonly AbstractFile VideoMaster = new VideoPreservationFile(new UnknownFile(MkvMasterFileName));

        private static readonly List<Device> SignalChain = new List<Device>();
        private static readonly DigitalAudioFile PresIntProvenance = new DigitalAudioFile
        {
            AnalogOutputVoltage = "AnalogOutputVoltage value",
            BextFile = "BextFile value",
            Comment = "Comment value",
            CreatedBy = "creator",
            Gain = "Gain value",
            Peak = "Peak value",
            ReferenceFluxivity = "RF value",
            Rolloff = "RollOff Value",
            Filename = PresIntSide1Filename,
            DateDigitized = new DateTime(2019, 6, 12),
            SignalChain = SignalChain,
            SpeedUsed = "SpeedUsed value",
            StylusSize = "StylusSize value",
            Turnover = "Turnover value"
        };

        private static readonly object[] AdjustFormatCases =
        {
            new object [] { MediaFormats.LacquerDisc, Side1ZipMaster, MediaFormats.LacquerDiscIrene, "It should convert format to LacquerDiscIrene when master is .zip file"},
            new object [] { MediaFormats.LacquerDisc, Side1WavMaster, MediaFormats.LacquerDisc, "It should not convert format to LacquerDiscIrene when master is .wav file"},
            new object [] { MediaFormats.EightMillimeterVideo, VideoMaster, MediaFormats.EightMillimeterVideo, "It should not convert format to LacquerDiscIrene when format is video format"}
        };

        [TestCaseSource(nameof(AdjustFormatCases))]
        public void ItShouldAdjustFormatToLacquerDiscIreneCorrectly(IMediaFormat originalFormat, AbstractFile master, IMediaFormat expected, string description)
        {
            var metadata = new AudioPodMetadata
            {
                Format = originalFormat
            };

            var models = new List<AbstractFile>
            {
                master
            };

            var result = Provider.AdjustMediaFormat(metadata, models);
            Assert.That(metadata.Format, Is.EqualTo(expected), description);
        }

        [Test]
        public void ItShouldAdjustDigitalProvenanceDataIfIreneMasterPresent()
        {
            var masters = new List<AbstractFile>
            {
                Side1ZipMaster, Side1PresIntMaster
            };

            var provenances = new List<AbstractDigitalFile>
            {
                PresIntProvenance
            };

            var metadata = new AudioPodMetadata
            {
                FileProvenances = provenances
            };

            var results = Provider.AdjustDigitalProvenanceData(metadata, masters);

            Assert.That(results.FileProvenances.Count, Is.EqualTo(2));

            var zipProvenance = results.FileProvenances.SingleOrDefault(p => p.Filename.Equals(ZipMasterSide1FileName)) as DigitalAudioFile;
            Assert.That(zipProvenance, Is.Not.Null, "Zip master provenance should be present");

            Assert.That(zipProvenance?.AnalogOutputVoltage, Is.EqualTo(PresIntProvenance.AnalogOutputVoltage), "AnalogOutputVoltage should be set correctly");
            Assert.That(zipProvenance?.BextFile, Is.Null, "It should not set BestFile value");
            Assert.That(zipProvenance?.Comment, Is.EqualTo(PresIntProvenance.Comment), "Comment should be set correctly");
            Assert.That(zipProvenance?.CreatedBy, Is.EqualTo(PresIntProvenance.CreatedBy), "CreatedBy should be set correctly");
            Assert.That(zipProvenance?.DateDigitized, Is.EqualTo(PresIntProvenance.DateDigitized), "DateDigitized should be set correctly");
            Assert.That(zipProvenance?.Gain, Is.EqualTo(PresIntProvenance.Gain), "Gain should be set correctly");
            Assert.That(zipProvenance?.Peak, Is.EqualTo(PresIntProvenance.Peak), "Peak should be set correctly");
            Assert.That(zipProvenance?.ReferenceFluxivity, Is.EqualTo(PresIntProvenance.ReferenceFluxivity), "ReferenceFluxivity should be set correctly");
            Assert.That(zipProvenance?.Rolloff, Is.EqualTo(PresIntProvenance.Rolloff), "Rolloff should be set correctly");
            Assert.That(zipProvenance?.SignalChain, Is.EqualTo(PresIntProvenance.SignalChain), "SignalChain should be set correctly");
            Assert.That(zipProvenance?.SpeedUsed, Is.EqualTo(PresIntProvenance.SpeedUsed), "SpeedUsed should be set correctly");
            Assert.That(zipProvenance?.StylusSize, Is.EqualTo(PresIntProvenance.StylusSize), "StylusSize should be set correctly");
            Assert.That(zipProvenance?.Turnover, Is.EqualTo(PresIntProvenance.Turnover), "Turnover should be set correctly");
        }

        [Test]
        public void ItShouldNotAddZipProvenanceIfAlreadyPresent()
        {
            var masters = new List<AbstractFile>
            {
                Side1ZipMaster, Side1PresIntMaster
            };

            var provenances = new List<AbstractDigitalFile>
            {
                PresIntProvenance,
                new DigitalAudioFile {Filename = ZipMasterSide1FileName}
            };

            var metadata = new AudioPodMetadata
            {
                FileProvenances = provenances
            };

            var results = Provider.AdjustDigitalProvenanceData(metadata, masters);

            Assert.That(results.FileProvenances.Count, Is.EqualTo(2));
        }

        [Test]
        public void ItShouldNotAddZipProvenanceIfNoZipMasterPresent()
        {
            var masters = new List<AbstractFile>
            {
                Side1PresIntMaster
            };

            var provenances = new List<AbstractDigitalFile>
            {
                PresIntProvenance,
            };

            var metadata = new AudioPodMetadata
            {
                FileProvenances = provenances
            };

            var results = Provider.AdjustDigitalProvenanceData(metadata, masters);

            Assert.That(results.FileProvenances.Count, Is.EqualTo(1));
        }

        [Test]
        public void ItShouldAddProvenancesCorrectlyWhenMultipleSidesPresent()
        {
            var masters = new List<AbstractFile>
            {
                Side1ZipMaster, Side1PresIntMaster, Side2ZipMaster, Side2PresIntMaster
            };

            var provenances = new List<AbstractDigitalFile>
            {
                new DigitalAudioFile {Filename = PresIntSide1Filename},
                new DigitalAudioFile {Filename = PresIntSide2Filename}
            };

            var metadata = new AudioPodMetadata
            {
                FileProvenances = provenances
            };

            var results = Provider.AdjustDigitalProvenanceData(metadata, masters);

            Assert.That(results.FileProvenances.Count, Is.EqualTo(4));
            var size1ZipProvenance = results.FileProvenances.SingleOrDefault(p => p.Filename.Equals(ZipMasterSide1FileName));
            var size2ZipProvenance = results.FileProvenances.SingleOrDefault(p => p.Filename.Equals(ZipMasterSide2FileName));

            Assert.That(size1ZipProvenance, Is.Not.Null);
            Assert.That(size2ZipProvenance, Is.Not.Null);
        }

        [Test]
        public void AdjustProvenancesShouldThroughExceptionIfNoMatchingPresIntProvenanceFound()
        {

            var masters = new List<AbstractFile>
            {
                Side1ZipMaster, Side1PresIntMaster
            };

            var metadata = new AudioPodMetadata
            {
                FileProvenances = new List<AbstractDigitalFile>()
            };

            var issue = Assert.Throws<PodMetadataException>(() => Provider.AdjustDigitalProvenanceData(metadata, masters));
            Assert.That(issue.Message, 
                Is.EqualTo($"Could not backfill .zip (Irene) master provenance; No provenance present for {PresIntSide1Filename}"));

        }
    }
}
