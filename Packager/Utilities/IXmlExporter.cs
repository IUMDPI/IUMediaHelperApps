using System.Text;

namespace Packager.Utilities
{
    public interface IXmlExporter
    {
        void ExportToFile(object o, string path);
    }
}