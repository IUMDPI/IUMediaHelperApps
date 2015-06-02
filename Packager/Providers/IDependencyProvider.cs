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
    }
}