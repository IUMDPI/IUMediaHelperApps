using System.Xml.Linq;
using Packager.Providers;

namespace Packager.Deserializers
{
    public interface IImportable
    {
        void ImportFromXml(XElement element, ILookupsProvider lookupsProvider);
    }
}