namespace Packager.Utilities.Xml
{
    public interface IXmlExporter
    {
        void ExportToFile(object o, string path);
        T ImportFromFile<T>(string path) where T: class;
    }
}