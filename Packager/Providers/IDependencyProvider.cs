using Packager.Models;
using Packager.Observers;
using Packager.Utilities;

namespace Packager.Providers
{
    public interface IDependencyProvider
    {
        IHasher Hasher { get; }
        IXmlExporter XmlExporter { get; }
        IDirectoryProvider DirectoryProvider { get; }
        IFileProvider FileProvider { get; }
        IProcessRunner ProcessRunner { get; }
        IMetadataGenerator MetadataGenerator { get; }
        IProgramSettings ProgramSettings { get; }
        IObserverCollection Observers { get; }
        IPodMetadataProvider MetadataProvider { get; }
        ILookupsProvider LookupsProvider { get; }
        IBextProcessor BextProcessor { get; }
    }
}