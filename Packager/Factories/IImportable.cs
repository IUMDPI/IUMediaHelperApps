using System.Xml.Linq;

namespace Packager.Factories
{
    public interface IImportable
    {
        void ImportFromXml(XElement element, IImportableFactory factory);
    }
}