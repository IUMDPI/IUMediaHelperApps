using System.Linq;
using System.Xml.Linq;

namespace Packager.Extensions
{
    public static class XElementExtensions
    {
        public static string GetValueFromDescendent(this XElement parent, string elementLocalName, string defaultValue = "")
        {
            var element = parent.Descendants().FirstOrDefault(e => e.Name.LocalName.Equals(elementLocalName));
            return element == null ? defaultValue : element.Value;
        }

        public static string[] GetGroupValuesFromDescendent(this XElement parent, string elementLocalName)
        {
            var element = parent.Descendants().FirstOrDefault(e => e.Name.LocalName.Equals(elementLocalName));
            return element == null
                ? new string[0]
                : element.Descendants().Where(e => e.Value.ToLowerInvariant().Equals("true"))
                    .Select(e => e.Name.LocalName).ToArray();
        }
    }
}