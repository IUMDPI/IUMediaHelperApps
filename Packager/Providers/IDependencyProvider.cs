﻿using Packager.Factories;
using Packager.Models;
using Packager.Observers;
using Packager.Utilities;
using Packager.Validators;

namespace Packager.Providers
{
    public interface IDependencyProvider
    {
        IHasher Hasher { get; }
        IXmlExporter XmlExporter { get; }
        IDirectoryProvider DirectoryProvider { get; }
        IFileProvider FileProvider { get; }
        IProcessRunner ProcessRunner { get; }
        ICarrierDataFactory MetadataGenerator { get; }
        IProgramSettings ProgramSettings { get; }
        IObserverCollection Observers { get; }
        IPodMetadataProvider MetadataProvider { get; }
        ISystemInfoProvider SystemInfoProvider { get; }
        IBextProcessor BextProcessor { get; }
        IAudioFFMPEGRunner AudioFFMPEGRunner { get; }
        IVideoFFMPEGRunner VideoFFMPEGRunner { get; }

        IBwfMetaEditRunner MetaEditRunner { get; }
        IEmailSender EmailSender { get; }
        ISideDataFactory SideDataFactory { get; }
        IIngestDataFactory IngestDataFactory { get; }
        IValidatorCollection ValidatorCollection { get; }
        ISuccessFolderCleaner SuccessFolderCleaner { get; }

        IBextMetadataFactory AudioMetadataFactory { get; }
        IVideoMetadataFactory VideoMetadataFactory { get; }
        ILookupsProvider LookupsProvider { get; }
    }
}