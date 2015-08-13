using System.Collections.Generic;
using Packager.Factories;
using Packager.Models;
using Packager.Observers;
using Packager.Utilities;
using Packager.Validators;
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
            MetadataProvider = new PodMetadataProvider(ProgramSettings, LookupsProvider);
            BextProcessor = new BextProcessor(programSettings, ProcessRunner, XmlExporter, Observers,
                new BwfMetaEditResultsVerifier(), new ConformancePointDocumentFactory());
            EmailSender = new EmailSender(FileProvider, ProgramSettings.SmtpServer);
            ValidatorCollection = new StandardValidatorCollection
            {
                new ValueRequiredValidator(),
                new DirectoryExistsValidator(DirectoryProvider),
                new FileExistsValidator(FileProvider),
                new UriValidator()
            };
        }

        public ISystemInfoProvider SystemInfoProvider { get; private set; }
        public IHasher Hasher { get; private set; }
        public IXmlExporter XmlExporter { get; private set; }
        public IDirectoryProvider DirectoryProvider { get; private set; }
        public IFileProvider FileProvider { get; private set; }
        public IProcessRunner ProcessRunner { get; private set; }
        public ICarrierDataFactory MetadataGenerator { get; private set; }
        public IProgramSettings ProgramSettings { get; private set; }
        public IObserverCollection Observers { get; private set; }
        public IPodMetadataProvider MetadataProvider { get; private set; }
        public ILookupsProvider LookupsProvider { get; private set; }
        public IBextProcessor BextProcessor { get; private set; }
        public IEmailSender EmailSender { get; private set; }
        public ISideDataFactory SideDataFactory { get; private set; }
        public IIngestDataFactory IngestDataFactory { get; private set; }
        public IValidatorCollection ValidatorCollection { get; private set; }
    }
}