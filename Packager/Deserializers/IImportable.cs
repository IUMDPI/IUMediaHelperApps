using System.Xml.Linq;

namespace Packager.Deserializers
{
    public interface IImportable
    {
        void ImportFromXml(XElement element);
    }
}