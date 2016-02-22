using System.Threading;
using Packager.Factories;
using Packager.Models;
using Packager.Models.PodMetadataModels;
using Packager.Models.SettingsModels;
using Packager.Observers;
using Packager.Utilities;
using Packager.Utilities.Bext;
using Packager.Utilities.Email;
using Packager.Utilities.FileSystem;
using Packager.Utilities.Hashing;
using Packager.Utilities.ProcessRunners;
using Packager.Utilities.Xml;
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
        ICarrierDataFactory<AudioPodMetadata> AudioCarrierDataFactory { get; }
        ICarrierDataFactory<VideoPodMetadata> VideoCarrierDataFactory { get; }
        IProgramSettings ProgramSettings { get; }
        IImportableFactory ImportableFactory { get; }
        IObserverCollection Observers { get; }
        IPodMetadataProvider MetadataProvider { get; }
        ISystemInfoProvider SystemInfoProvider { get; }
        IBextProcessor BextProcessor { get; }
        IFFMPEGRunner AudioFFMPEGRunner { get; }
        IFFMPEGRunner VideoFFMPEGRunner { get; }
        IBwfMetaEditRunner MetaEditRunner { get; }
        IEmailSender EmailSender { get; }
        IValidatorCollection ValidatorCollection { get; }
        ISuccessFolderCleaner SuccessFolderCleaner { get; }
        IEmbeddedMetadataFactory<AudioPodMetadata> AudioMetadataFactory { get; }
        IEmbeddedMetadataFactory<VideoPodMetadata> VideoMetadataFactory { get; }
        IFFProbeRunner FFProbeRunner { get; }
        
        CancellationTokenSource CancellationTokenSource { get; }
    }
}