using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using NSubstitute;
using NUnit.Framework;
using Packager.Factories;
using Packager.Models;
using Packager.Models.EmbeddedMetadataModels;
using Packager.Models.FileModels;
using Packager.Models.PodMetadataModels;
using Packager.Models.SettingsModels;
using Packager.Observers;
using Packager.Processors;
using Packager.Providers;
using Packager.Utilities;
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
        protected ICarrierDataFactory MetadataGenerator { get; set; }
        protected IBextProcessor BextProcessor { get; set; }
        protected IFFMPEGRunner FFMPEGRunner { get; set; }
        protected string ExpectedProcessingDirectory => Path.Combine(ProcessingRoot, ExpectedObjectFolderName);
        protected string ExpectedOriginalsDirectory => Path.Combine(ExpectedProcessingDirectory, "Originals");
        protected string ExpectedObjectFolderName { get; set; }
      
        protected abstract void DoCustomSetup();

        protected IEmbeddedMetadataFactory<AudioPodMetadata> AudioMetadataFactory { get; set; }
        protected IEmbeddedMetadataFactory<VideoPodMetadata> VideoMetadataFactory { get; set; }

        protected string PreservationFileName { get; set; }
        protected string PreservationIntermediateFileName { get; set; }
        protected string ProductionFileName { get; set; }
        protected string AccessFileName { get; set; }

        protected ObjectFileModel PresObjectFileModel { get; set; }
        protected ObjectFileModel ProdObjectFileModel { get; set; }
        protected ObjectFileModel PresIntObjectFileModel { get; set; }
        protected ObjectFileModel AccessObjectFileModel { get; set; }
        
        protected string XmlManifestFileName { get; set; }
 
        public ValidationResult Result { get; set; }

        protected IGrouping<string, AbstractFileModel> GetGrouping(IEnumerable<AbstractFileModel> models)
        {
            return models.GroupBy(m => m.BarCode).First();
        }

        protected List<AbstractFileModel> ModelList { get; set; }

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
            AudioMetadataFactory.Generate(Arg.Any<IEnumerable<ObjectFileModel>>(), Arg.Any<ObjectFileModel>(),Arg.Any<AudioPodMetadata>())
                .Returns(new EmbeddedAudioMetadata());

            VideoMetadataFactory = Substitute.For<IEmbeddedMetadataFactory<VideoPodMetadata>>();
            VideoMetadataFactory.Generate(Arg.Any<IEnumerable<ObjectFileModel>>(), Arg.Is<ObjectFileModel>(a=>a.IsMezzanineVersion()),Arg.Any<VideoPodMetadata>())
                .Returns(new EmbeddedVideoMezzanineMetadata());
            VideoMetadataFactory.Generate(Arg.Any<IEnumerable<ObjectFileModel>>(), Arg.Is<ObjectFileModel>(a => a.IsPreservationVersion() || a.IsPreservationIntermediateVersion()), Arg.Any<VideoPodMetadata>())
                .Returns(new EmbeddedVideoPreservationMetadata());

            DirectoryProvider = Substitute.For<IDirectoryProvider>();
            FileProvider = Substitute.For<IFileProvider>();
            Hasher = Substitute.For<IHasher>();
            ProcessRunner = Substitute.For<IProcessRunner>();
            XmlExporter = Substitute.For<IXmlExporter>();
            Observers = Substitute.For<IObserverCollection>();
            MetadataProvider = Substitute.For<IPodMetadataProvider>();
            MetadataGenerator = Substitute.For<ICarrierDataFactory>();
            BextProcessor = Substitute.For<IBextProcessor>();
            FFMPEGRunner = Substitute.For<IFFMPEGRunner>();
          
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
            DependencyProvider.MetadataGenerator.Returns(MetadataGenerator);
            DependencyProvider.AudioFFMPEGRunner.Returns(FFMPEGRunner);
            DependencyProvider.VideoFFMPEGRunner.Returns(FFMPEGRunner);
            DependencyProvider.AudioMetadataFactory.Returns(AudioMetadataFactory);
            DependencyProvider.VideoMetadataFactory.Returns(VideoMetadataFactory);

            DoCustomSetup();

            Result =  await Processor.ProcessFile(GetGrouping(ModelList));
            
        }

       
    }
}