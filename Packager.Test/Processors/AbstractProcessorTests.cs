using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using NSubstitute;
using NUnit.Framework;
using Packager.Factories;
using Packager.Models;
using Packager.Models.BextModels;
using Packager.Models.FileModels;
using Packager.Models.PodMetadataModels;
using Packager.Observers;
using Packager.Processors;
using Packager.Providers;
using Packager.Utilities;
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
        protected ConsolidatedPodMetadata Metadata { get; set; }
        protected abstract void DoCustomSetup();

        protected IBextMetadataFactory AudioMetadataFactory { get; set; }

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

            AudioMetadataFactory = Substitute.For<IBextMetadataFactory>();

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
            FFMPEGRunner.CreateAccessDerivative(Arg.Any<ObjectFileModel>()).Returns(x => Task.FromResult(x.Arg<ObjectFileModel>()
                .ToAudioAccessFileModel()));
            FFMPEGRunner.CreateProductionDerivative(Arg.Any<ObjectFileModel>(), Arg.Any<ObjectFileModel>(), Arg.Any<BextMetadata>()).Returns(x => Task.FromResult(x.ArgAt<ObjectFileModel>(1)
                .ToProductionFileModel()));

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
            DependencyProvider.FFMPEGRunner.Returns(FFMPEGRunner);
            DependencyProvider.AudioMetadataFactory.Returns(AudioMetadataFactory);

            DoCustomSetup();

            Result =  await Processor.ProcessFile(GetGrouping(ModelList));
            
        }

       
    }
}