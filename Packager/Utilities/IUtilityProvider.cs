namespace Packager.Utilities
{
    public interface IUtilityProvider
    {
        IExcelImporter CarrierDataExcelImporter { get; }
        IBextDataProvider BextDataProvider { get; }
        IHasher Hasher { get; }
        IUserInfoResolver UserInfoResolver { get; }
        IXmlExporter XmlExporter { get; }
    }
}