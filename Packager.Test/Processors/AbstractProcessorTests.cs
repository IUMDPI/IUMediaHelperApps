using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using NSubstitute;
using NUnit.Framework;
using Packager.Extensions;
using Packager.Factories;
using Packager.Models.EmbeddedMetadataModels;
using Packager.Models.FileModels;
using Packager.Models.PodMetadataModels;
using Packager.Models.SettingsModels;
using Packager.Observers;
using Packager.Processors;
using Packager.Providers;
using Packager.Utilities.Bext;
using Packager.Utilities.Hashing;
using Packager.Utilities.Process;
using Packager.Utilities.Xml;
using Packager.Validators;

namespace Packager.Test.Processors
{
    public abstract class AbstractProcessorTests
    {
        protected const string ProjectCode = "MDPI";
        protected const string Barcode = "4890764553278906";
        protected const string InputDirectory = "work";
        protected const string DropBoxRoot = "dropbox";
        protected const string ErrorDirectory = "error";
        protected const string ProcessingRoot = "processing";

        protected IProgramSettings ProgramSettings { get; set; }
        protected IDependencyProvider DependencyProvider { get; set; }
        protected IDirectoryProvider DirectoryProvider { get; set; }
        protected IFileProvider FileProvider { get; set; }
        protected IHasher Hasher { get; set; }
        protected IProcessRunner ProcessRunner { get; set; }
        protected IXmlExporter XmlExporter { get; set; }
        protected IObserverCollection Observers { get; set; }
        protected IProcessor Processor { get; set; }
        protected IPodMetadataProvider MetadataProvider { get; set; }
        protected ICarrierDataFactory<AudioPodMetadata> AudioCarrierDataFactory { get; set; }
        protected ICarrierDataFactory<VideoPodMetadata> VideoCarrierDataFactory { get; set; }
        protected IBextProcessor BextProcessor { get; set; }
        protected IFFMPEGRunner FFMPEGRunner { get; set; }
        protected IFFProbeRunner FFProbeRunner { get; set; }

        protected string ExpectedProcessingDirectory => Path.Combine(ProcessingRoot, ExpectedObjectFolderName);
        protected string ExpectedOriginalsDirectory => Path.Combine(ExpectedProcessingDirectory, "Originals");
        protected string ExpectedObjectFolderName { get; set; }

        protected IEmbeddedMetadataFactory<AudioPodMetadata> AudioMetadataFactory { get; set; }
        protected IEmbeddedMetadataFactory<VideoPodMetadata> VideoMetadataFactory { get; set; }

        protected string PreservationFileName { get; set; }
        protected string PreservationIntermediateFileName { get; set; }
        protected string ProductionFileName { get; set; }
        protected string AccessFileName { get; set; }

        protected AbstractFile PresAbstractFile { get; set; }
        protected AbstractFile ProdAbstractFile { get; set; }
        protected AbstractFile PresIntAbstractFile { get; set; }
        protected AbstractFile AccessAbstractFile { get; set; }

        protected string XmlManifestFileName { get; set; }

        public ValidationResult Result { get; set; }

        protected List<AbstractFile> ModelList { get; set; }

        protected abstract void DoCustomSetup();

        protected IGrouping<string, AbstractFile> GetGrouping(IEnumerable<AbstractFile> models)
        {
            return models.GroupBy(m => m.BarCode).First();
        }

        [SetUp]
        public virtual async void BeforeEach()
        {
            ProgramSettings = Substitute.For<IProgramSettings>();
            ProgramSettings.ProjectCode.Returns(ProjectCode);
            ProgramSettings.InputDirectory.Returns(InputDirectory);
            ProgramSettings.DropBoxDirectoryName.Returns(DropBoxRoot);
            ProgramSettings.ErrorDirectoryName.Returns(ErrorDirectory);
            ProgramSettings.ProcessingDirectory.Returns(ProcessingRoot);

            AudioMetadataFactory = Substitute.For<IEmbeddedMetadataFactory<AudioPodMetadata>>();
            AudioMetadataFactory.Generate(Arg.Any<IEnumerable<AbstractFile>>(), Arg.Any<AbstractFile>(),Arg.Any<AudioPodMetadata>())
                .Returns(new EmbeddedAudioMetadata());

            VideoMetadataFactory = Substitute.For<IEmbeddedMetadataFactory<VideoPodMetadata>>();
            VideoMetadataFactory.Generate(Arg.Any<IEnumerable<AbstractFile>>(),
                Arg.Is<AbstractFile>(a => a.IsMezzanineVersion()), Arg.Any<VideoPodMetadata>())
                .Returns(new EmbeddedVideoMezzanineMetadata());
            VideoMetadataFactory.Generate(Arg.Any<IEnumerable<AbstractFile>>(),
                Arg.Is<AbstractFile>(a => a.IsPreservationVersion() || a.IsPreservationIntermediateVersion()),
                Arg.Any<VideoPodMetadata>())
                .Returns(new EmbeddedVideoPreservationMetadata());

            DirectoryProvider = Substitute.For<IDirectoryProvider>();
            FileProvider = Substitute.For<IFileProvider>();
            Hasher = Substitute.For<IHasher>();
            ProcessRunner = Substitute.For<IProcessRunner>();
            XmlExporter = Substitute.For<IXmlExporter>();
            Observers = Substitute.For<IObserverCollection>();
            MetadataProvider = Substitute.For<IPodMetadataProvider>();
            AudioCarrierDataFactory = Substitute.For<ICarrierDataFactory<AudioPodMetadata>>();
            VideoCarrierDataFactory = Substitute.For<ICarrierDataFactory<VideoPodMetadata>>();
            BextProcessor = Substitute.For<IBextProcessor>();
            FFMPEGRunner = Substitute.For<IFFMPEGRunner>();
            FFProbeRunner = Substitute.For<IFFProbeRunner>();
            FFProbeRunner.GenerateQualityControlFile(Arg.Any<AbstractFile>())
                .Returns(a => Task.FromResult(new QualityControlFile(a.Arg<AbstractFile>())));

            DependencyProvider = Substitute.For<IDependencyProvider>();
            DependencyProvider.FileProvider.Returns(FileProvider);
            DependencyProvider.DirectoryProvider.Returns(DirectoryProvider);
            DependencyProvider.Hasher.Returns(Hasher);
            DependencyProvider.MetadataProvider.Returns(MetadataProvider);
            DependencyProvider.ProcessRunner.Returns(ProcessRunner);
            DependencyProvider.ProgramSettings.Returns(ProgramSettings);
            DependencyProvider.Observers.Returns(Observers);
            DependencyProvider.XmlExporter.Returns(XmlExporter);
            DependencyProvider.BextProcessor.Returns(BextProcessor);
            DependencyProvider.AudioCarrierDataFactory.Returns(AudioCarrierDataFactory);
            DependencyProvider.VideoCarrierDataFactory.Returns(VideoCarrierDataFactory);
            DependencyProvider.AudioFFMPEGRunner.Returns(FFMPEGRunner);
            DependencyProvider.VideoFFMPEGRunner.Returns(FFMPEGRunner);
            DependencyProvider.AudioMetadataFactory.Returns(AudioMetadataFactory);
            DependencyProvider.VideoMetadataFactory.Returns(VideoMetadataFactory);
            DependencyProvider.FFProbeRunner.Returns(FFProbeRunner);

            DoCustomSetup();

            Result = await Processor.ProcessFile(GetGrouping(ModelList));
        }
    }
}