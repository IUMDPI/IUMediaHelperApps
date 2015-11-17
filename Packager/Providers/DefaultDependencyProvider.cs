using System;
using Packager.Deserializers;
using Packager.Factories;
using Packager.Models;
using Packager.Observers;
using Packager.Utilities;
using Packager.Validators;
using Packager.Validators.Attributes;
using Packager.Verifiers;
using RestSharp;
using RestSharp.Authenticators;

namespace Packager.Providers
{
    public class DefaultDependencyProvider : IDependencyProvider
    {
        public DefaultDependencyProvider(IProgramSettings programSettings)
        {
            Hasher = new Hasher(programSettings.ProcessingDirectory);
            XmlExporter = new XmlExporter();
            DirectoryProvider = new DirectoryProvider();
            FileProvider = new FileProvider();
            ProcessRunner = new ProcessRunner();
            ProgramSettings = programSettings;
            IngestDataFactory = new IngestDataFactory();
            SideDataFactory = new SideDataFactory(IngestDataFactory);
            MetadataGenerator = new CarrierDataFactory(SideDataFactory);
            SystemInfoProvider = new SystemInfoProvider(programSettings.LogDirectoryName);
            Observers = new ObserverCollection();
            AudioMetadataFactory = new BextMetadataFactory();
            VideoMetadataFactory = new VideoMetadataFactory();
            MetaEditRunner = new BwfMetaEditRunner(ProcessRunner, programSettings.BwfMetaEditPath, programSettings.ProcessingDirectory);
            BextProcessor = new BextProcessor(MetaEditRunner, Observers, new BwfMetaEditResultsVerifier());
            AudioFFMPEGRunner = new AudioFFMPEGRunner(ProgramSettings, ProcessRunner, Observers, FileProvider, Hasher);
            VideoFFMPEGRunner = new VideoFFMPEGRunner(ProgramSettings, ProcessRunner, Observers, FileProvider, Hasher);
            EmailSender = new EmailSender(FileProvider, ProgramSettings.SmtpServer);
            LookupsProvider = new AppConfigLookupsProvider();
            ValidatorCollection = new StandardValidatorCollection
            {
                new ValueRequiredValidator(),
                new DirectoryExistsValidator(DirectoryProvider),
                new FileExistsValidator(FileProvider),
                new UriValidator(),
                new MembersValidator()
            };
            MetadataProvider = new PodMetadataProvider(GetRestClient(ProgramSettings, LookupsProvider), Observers, ValidatorCollection);
            SuccessFolderCleaner = new SuccessFolderCleaner(DirectoryProvider, programSettings.SuccessDirectoryName,
                new TimeSpan(programSettings.DeleteSuccessfulObjectsAfterDays, 0, 0, 0), Observers);
        }
        
        private static IRestClient GetRestClient(IProgramSettings programSettings, ILookupsProvider lookupsProvider)
        {
            var result = new RestClient(programSettings.WebServiceUrl)
            {
                Authenticator =
                    new HttpBasicAuthenticator(programSettings.PodAuth.UserName, programSettings.PodAuth.Password)
            };

            result.AddHandler("application/xml", new PodResultDeserializer(lookupsProvider));
            return result;

        }

        [ValidateObject]
        public ISystemInfoProvider SystemInfoProvider { get; }

        [ValidateObject]
        public IHasher Hasher { get; }

        [ValidateObject]
        public IXmlExporter XmlExporter { get; }

        [ValidateObject]
        public IDirectoryProvider DirectoryProvider { get; }

        [ValidateObject]
        public IFileProvider FileProvider { get; }

        [ValidateObject]
        public IProcessRunner ProcessRunner { get; }

        [ValidateObject]
        public ICarrierDataFactory MetadataGenerator { get; }

        [ValidateObject]
        public IProgramSettings ProgramSettings { get; }

        [ValidateObject]
        public IObserverCollection Observers { get; }

        [ValidateObject]
        public IPodMetadataProvider MetadataProvider { get; }

        [ValidateObject]
        public IBextProcessor BextProcessor { get; }

        [ValidateObject]
        public IFFMPEGRunner AudioFFMPEGRunner { get; }

        public IFFMPEGRunner VideoFFMPEGRunner { get; }

        [ValidateObject]
        public IEmailSender EmailSender { get; }

        [ValidateObject]
        public ISideDataFactory SideDataFactory { get; }

        [ValidateObject]
        public IIngestDataFactory IngestDataFactory { get; }

        public IValidatorCollection ValidatorCollection { get; }
        public ISuccessFolderCleaner SuccessFolderCleaner { get; }
        public IBextMetadataFactory AudioMetadataFactory { get; }
        public IVideoMetadataFactory VideoMetadataFactory { get; }

        [ValidateObject]
        public ILookupsProvider LookupsProvider { get; }

        public IBwfMetaEditRunner MetaEditRunner { get; }
    }
}