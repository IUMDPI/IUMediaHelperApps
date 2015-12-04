﻿using System;
using Packager.Deserializers;
using Packager.Factories;
using Packager.Models;
using Packager.Models.PodMetadataModels;
using Packager.Observers;
using Packager.Utilities;
using Packager.Utilities.Bext;
using Packager.Utilities.Email;
using Packager.Utilities.FileSystem;
using Packager.Utilities.Hashing;
using Packager.Utilities.Process;
using Packager.Utilities.Xml;
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
            AudioMetadataFactory = new EmbeddedAudioMetadataFactory();
            VideoMetadataFactory = new EmbeddedVideoMetadataFactory();
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
            FFProbeRunner = new FFProbeRunner(ProgramSettings, ProcessRunner, FileProvider, Observers);
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
        public IEmbeddedMetadataFactory<AudioPodMetadata> AudioMetadataFactory { get; }
        public IEmbeddedMetadataFactory<VideoPodMetadata> VideoMetadataFactory { get; }

        [ValidateObject]
        public ILookupsProvider LookupsProvider { get; }

        public IFFProbeRunner FFProbeRunner { get; }

        public IBwfMetaEditRunner MetaEditRunner { get; }
    }
}