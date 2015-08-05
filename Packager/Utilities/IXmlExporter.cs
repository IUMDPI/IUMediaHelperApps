namespace Packager.Utilities
{
    public interface IXmlExporter
    {
        string GenerateXml(object o);
        void ExportToFile(object o, string path);
    }
}