using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace Packager.Utilities
{
    internal interface IXmlExporter
    {
        string GenerateXml(object o);
    }

    internal class XmlExporter : IXmlExporter
    {
        public string GenerateXml(object o)
        {
            var serializer = new XmlSerializer(o.GetType());

            // code borrow
            using (var textWriter = new StringWriter())
            {
                using (var xmlWriter = XmlWriter.Create(textWriter))
                {
                    serializer.Serialize(xmlWriter, o);
                }
                return textWriter.ToString();
            }
        }
    }
}