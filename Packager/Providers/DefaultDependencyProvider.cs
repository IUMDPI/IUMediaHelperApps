﻿using System;
using System.Threading;
using Packager.Deserializers;
using Packager.Factories;
using Packager.Models.PodMetadataModels;
using Packager.Models.SettingsModels;
using Packager.Observers;
using Packager.Utilities.Bext;
using Packager.Utilities.Email;
using Packager.Utilities.FileSystem;
using Packager.Utilities.Hashing;
using Packager.Utilities.ProcessRunners;
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
            ProgramSettings = programSettings;
            Hasher = new Hasher(ProgramSettings);
            XmlExporter = new XmlExporter();
            DirectoryProvider = new DirectoryProvider();
            FileProvider = new FileProvider();
            ProcessRunner = new ProcessRunner();
            IngestDataFactory = new IngestDataFactory();
            SideDataFactory = new SideDataFactory(IngestDataFactory);
            AudioCarrierDataFactory = new AudioCarrierDataFactory(SideDataFactory);
            VideoCarrierDataFactory = new VideoCarrierDataFactory(SideDataFactory);

            SystemInfoProvider = new SystemInfoProvider(ProgramSettings);
            Observers = new ObserverCollection();
            AudioMetadataFactory = new EmbeddedAudioMetadataFactory(ProgramSettings);
            VideoMetadataFactory = new EmbeddedVideoMetadataFactory(ProgramSettings);
            MetaEditRunner = new BwfMetaEditRunner(ProgramSettings, ProcessRunner);
            BextProcessor = new BextProcessor(MetaEditRunner, Observers, new BwfMetaEditResultsVerifier());
            EmailSender = new EmailSender(ProgramSettings, FileProvider);
            ImportableFactory = new ImportableFactory();
            ValidatorCollection = new StandardValidatorCollection
            {
                new ValueRequiredValidator(),
                new DirectoryExistsValidator(DirectoryProvider),
                new FileExistsValidator(FileProvider),
                new UriValidator(),
                new MembersValidator()
            };
            MetadataProvider = new PodMetadataProvider(GetRestClient(ProgramSettings, FileProvider, ImportableFactory),
                Observers,
                ValidatorCollection);
            SuccessFolderCleaner = new SuccessFolderCleaner(ProgramSettings, DirectoryProvider, Observers);
            FFProbeRunner = new FFProbeRunner(ProgramSettings, ProcessRunner, FileProvider, Observers);
            MediaInfoProvider = new MediaInfoProvider(ProgramSettings, FFProbeRunner);

            //AudioFFMPEGRunner = new AudioFFMPEGRunner(this);
            //VideoFFMPEGRunner = new VideoFFMPEGRunner(this);
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
        public ICarrierDataFactory<AudioPodMetadata> AudioCarrierDataFactory { get; }

        [ValidateObject]
        public ICarrierDataFactory<VideoPodMetadata> VideoCarrierDataFactory { get; }
        
        [ValidateObject]
        public IProgramSettings ProgramSettings { get; }

        [ValidateObject]
        public IImportableFactory ImportableFactory { get; }

        [ValidateObject]
        public IObserverCollection Observers { get; }

        [ValidateObject]
        public IPodMetadataProvider MetadataProvider { get; }

        [ValidateObject]
        public IBextProcessor BextProcessor { get; }

        [ValidateObject]
        public IFFMPEGRunner AudioFFMPEGRunner { get; }

        [ValidateObject]
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
        public IFFProbeRunner FFProbeRunner { get; }

        [ValidateObject]
        public IMediaInfoProvider MediaInfoProvider { get; }

        [ValidateObject]
        public IBwfMetaEditRunner MetaEditRunner { get; }

        private static IRestClient GetRestClient(IProgramSettings programSettings, IFileProvider fileProvider,
            IImportableFactory factory)
        {
            var podAuth = fileProvider.Deserialize<PodAuth>(programSettings.PodAuthFilePath);
            var result = new RestClient(programSettings.WebServiceUrl)
            {
                Authenticator = new HttpBasicAuthenticator(podAuth.UserName, podAuth.Password)
            };

            result.AddHandler("application/xml", new PodResultDeserializer(factory));
            return result;
        }
    }
}