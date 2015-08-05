using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace Packager.Utilities
{
    internal class XmlExporter : IXmlExporter
    {
        public string GenerateXml(object o)
        {
            var serializer = new XmlSerializer(o.GetType());
            var settings = new XmlWriterSettings { Indent = true };
            // code borrow
            using (var textWriter = new StringWriter())
            {
                using (var xmlWriter = XmlWriter.Create(textWriter, settings))
                {
                    serializer.Serialize(xmlWriter, o);
                }
                return textWriter.ToString();
            }
        }

        public void ExportToFile(object o, string path)
        {
            var xml = GenerateXml(o);
            File.WriteAllText(path, xml);

        }
    }
}