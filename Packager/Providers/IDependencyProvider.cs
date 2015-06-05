using System.Collections.Generic;
using Packager.Models;
using Packager.Observers;
using Packager.Utilities;

namespace Packager.Providers
{
    public interface IDependencyProvider
    {
        IExcelImporter CarrierDataExcelImporter { get; }
        IBextDataProvider BextDataProvider { get; }
        IHasher Hasher { get; }
        IUserInfoResolver UserInfoResolver { get; }
        IXmlExporter XmlExporter { get; }
        IDirectoryProvider DirectoryProvider { get; }
        IFileProvider FileProvider { get; }
        IProcessRunner ProcessRunner { get; }
        IMetadataGenerator MetadataGenerator { get; }
        IProgramSettings ProgramSettings { get; }
        List<IObserver> Observers { get; }
        IPodMetadataProvider MetadataProvider { get;}
    }
}