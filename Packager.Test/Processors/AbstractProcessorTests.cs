using System.Linq;
using NSubstitute;
using NUnit.Framework;
using Packager.Models;
using Packager.Models.FileModels;
using Packager.Models.PodMetadataModels;
using Packager.Observers;
using Packager.Processors;
using Packager.Providers;
using Packager.Utilities;

namespace Packager.Test.Processors
{
    public abstract class AbstractProcessorTests
    {
        protected const string ProjectCode = "MDPI";
        protected const string Barcode = "4890764553278906";
        protected const string InputDirectory = "work";
        protected const string DropBoxRoot = "dropbox";
        
        protected IProgramSettings ProgramSettings { get; set; }
        protected IDependencyProvider DependencyProvider { get; set; }
        protected IDirectoryProvider DirectoryProvider { get; set; }
        protected IFileProvider FileProvider { get; set; }
        protected IHasher Hasher { get; set; }
        protected IProcessRunner ProcessRunner { get; set; }
        protected IUserInfoResolver UserInfoResolver { get; set; }
        protected IXmlExporter XmlExporter { get; set; }
        protected IObserverCollection Observers { get; set; }
        protected IProcessor Processor { get; set; }
        protected IPodMetadataProvider MetadataProvider { get; set; }
        protected IMetadataGenerator MetadataGenerator { get; set; }
        protected ObjectFileModel PresObjectFileModel { get; set; }
        protected ObjectFileModel ProdObjectFileModel { get; set; }
        protected ObjectFileModel AccessObjectFileModel { get; set; }
        protected IBextProcessor BextProcessor { get; set; }
        protected IGrouping<string, AbstractFileModel> Grouping { get; set; }
        protected string ProcessingDirectory { get; set; }
        protected ConsolidatedPodMetadata Metadata { get; set; }
        protected abstract void DoCustomSetup();

        protected string ProductionFileName { get; set; }
        protected string PreservationFileName { get; set; }
        protected string AccessFileName { get; set; }
        protected string XmlManifestFileName { get; set; }

        [SetUp]
        public virtual async void BeforeEach()
        {
            ProgramSettings = Substitute.For<IProgramSettings>();
            ProgramSettings.ProjectCode.Returns(ProjectCode);
            ProgramSettings.InputDirectory.Returns(InputDirectory);
            ProgramSettings.DropBoxDirectoryName.Returns(DropBoxRoot);

            DirectoryProvider = Substitute.For<IDirectoryProvider>();
            FileProvider = Substitute.For<IFileProvider>();
            Hasher = Substitute.For<IHasher>();
            ProcessRunner = Substitute.For<IProcessRunner>();
            UserInfoResolver = Substitute.For<IUserInfoResolver>();
            XmlExporter = Substitute.For<IXmlExporter>();
            Observers = Substitute.For<IObserverCollection>();
            MetadataProvider = Substitute.For<IPodMetadataProvider>();
            MetadataGenerator = Substitute.For<IMetadataGenerator>();
            BextProcessor = Substitute.For<IBextProcessor>();

            DependencyProvider = Substitute.For<IDependencyProvider>();
            DependencyProvider.FileProvider.Returns(FileProvider);
            DependencyProvider.DirectoryProvider.Returns(DirectoryProvider);
            DependencyProvider.Hasher.Returns(Hasher);
            DependencyProvider.MetadataProvider.Returns(MetadataProvider);
            DependencyProvider.ProcessRunner.Returns(ProcessRunner);
            DependencyProvider.ProgramSettings.Returns(ProgramSettings);
            DependencyProvider.UserInfoResolver.Returns(UserInfoResolver);
            DependencyProvider.Observers.Returns(Observers);
            DependencyProvider.XmlExporter.Returns(XmlExporter);
            DependencyProvider.BextProcessor.Returns(BextProcessor);
            DependencyProvider.MetadataGenerator.Returns(MetadataGenerator);
            DoCustomSetup();

            await Processor.ProcessFile(Grouping);
        }
    }
}