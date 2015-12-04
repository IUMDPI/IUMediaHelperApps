namespace Packager.Utilities.Xml
{
    public interface IXmlExporter
    {
        void ExportToFile(object o, string path);
    }
}