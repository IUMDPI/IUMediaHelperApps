using System;
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
            LookupsProvider = new AppConfigLookupsProvider();
            MetaEditRunner = new BwfMetaEditRunner(ProcessRunner, programSettings.BwfMetaEditPath, programSettings.ProcessingDirectory, 
                programSettings.SuppressAudioMetadataFields, programSettings.UseAppendFlagForAudioMetadata);
            BextProcessor = new BextProcessor(MetaEditRunner, Observers, new BwfMetaEditResultsVerifier(), new BextMetadataFactory());
            FFMPEGRunner = new FFMPEGRunner(ProgramSettings.FFMPEGPath, ProgramSettings.ProcessingDirectory, ProcessRunner, Observers, FileProvider);
            EmailSender = new EmailSender(FileProvider, ProgramSettings.SmtpServer);
            ValidatorCollection = new StandardValidatorCollection
            {
                new ValueRequiredValidator(),
                new DirectoryExistsValidator(DirectoryProvider),
                new FileExistsValidator(FileProvider),
                new UriValidator(),
                new MembersValidator()
            };
            MetadataProvider = new PodMetadataProvider(ProgramSettings, LookupsProvider, Observers, ValidatorCollection);
            SuccessFolderCleaner = new SuccessFolderCleaner(DirectoryProvider, programSettings.SuccessDirectoryName, 
                new TimeSpan(programSettings.DeleteSuccessfulObjectsAfterDays,0,0,0), Observers);
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
        public ILookupsProvider LookupsProvider { get; }

        [ValidateObject]
        public IBextProcessor BextProcessor { get; }

        [ValidateObject]
        public IFFMPEGRunner FFMPEGRunner {get; }

        [ValidateObject]
        public IEmailSender EmailSender { get; }

        [ValidateObject]
        public ISideDataFactory SideDataFactory { get; }

        [ValidateObject]
        public IIngestDataFactory IngestDataFactory { get; }
        
        public IValidatorCollection ValidatorCollection { get; }
        public ISuccessFolderCleaner SuccessFolderCleaner { get; }

        public IBwfMetaEditRunner MetaEditRunner { get; }
    }
}