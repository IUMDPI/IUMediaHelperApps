using Packager.Factories;
using Packager.Models;
using Packager.Observers;
using Packager.Utilities;
using Packager.Validators;
using Packager.Validators.Attributes;
using Packager.Verifiers;

namespace Packager.Providers
{
    public class DefaultDependencyProvider : IDependencyProvider
    {
        public DefaultDependencyProvider(IProgramSettings programSettings)
        {
            Hasher = new Hasher();
            XmlExporter = new XmlExporter();
            DirectoryProvider = new DirectoryProvider();
            FileProvider = new FileProvider();
            ProcessRunner = new ProcessRunner();
            ProgramSettings = programSettings;
            IngestDataFactory = new IngestDataFactory();
            SideDataFactory = new SideDataFactory(Hasher, IngestDataFactory);
            MetadataGenerator = new CarrierDataFactory(SideDataFactory);
            SystemInfoProvider = new SystemInfoProvider(programSettings.LogDirectoryName);
            Observers = new ObserverCollection();
            LookupsProvider = new AppConfigLookupsProvider();
            BextProcessor = new BextProcessor(programSettings.BwfMetaEditPath, ProcessRunner, XmlExporter, Observers,
                new BwfMetaEditResultsVerifier(), new ConformancePointDocumentFactory());
            FFMPEGRunner = new IffmpegRunner(ProgramSettings.FFMPEGPath, ProgramSettings.ProcessingDirectory, ProcessRunner, Observers, FileProvider);
            EmailSender = new EmailSender(FileProvider, ProgramSettings.SmtpServer);
            ValidatorCollection = new StandardValidatorCollection
            {
                new ValueRequiredValidator(),
                new DirectoryExistsValidator(DirectoryProvider),
                new FileExistsValidator(FileProvider),
                new UriValidator()
            };
            MetadataProvider = new PodMetadataProvider(ProgramSettings, LookupsProvider, Observers, ValidatorCollection);
        }

        [ValidateObject]
        public ISystemInfoProvider SystemInfoProvider { get; private set; }

        [ValidateObject]
        public IHasher Hasher { get; private set; }

        [ValidateObject]
        public IXmlExporter XmlExporter { get; private set; }

        [ValidateObject]
        public IDirectoryProvider DirectoryProvider { get; private set; }
        
        [ValidateObject]
        public IFileProvider FileProvider { get; private set; }

        [ValidateObject]
        public IProcessRunner ProcessRunner { get; private set; }

        [ValidateObject]
        public ICarrierDataFactory MetadataGenerator { get; private set; }

        [ValidateObject]
        public IProgramSettings ProgramSettings { get; private set; }

        [ValidateObject]
        public IObserverCollection Observers { get; private set; }

        [ValidateObject]
        public IPodMetadataProvider MetadataProvider { get; private set; }

        [ValidateObject]
        public ILookupsProvider LookupsProvider { get; private set; }

        [ValidateObject]
        public IBextProcessor BextProcessor { get; private set; }

        [ValidateObject]
        public IFFMPEGRunner FFMPEGRunner {get; private set; }

        [ValidateObject]
        public IEmailSender EmailSender { get; private set; }

        [ValidateObject]
        public ISideDataFactory SideDataFactory { get; private set; }

        [ValidateObject]
        public IIngestDataFactory IngestDataFactory { get; private set; }
        
        public IValidatorCollection ValidatorCollection { get; private set; }
    }
}