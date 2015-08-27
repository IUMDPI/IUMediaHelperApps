using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace Packager.Utilities
{
    internal class XmlExporter : IXmlExporter
    {
  
        public void ExportToFile(object o, string path, Encoding encoding)
        {
            var settings = new XmlWriterSettings { Indent = true, Encoding = encoding};

            var xmlSerializer = new XmlSerializer(o.GetType());

            using (Stream stream = new FileStream(path, FileMode.Create))
            using (var xmlWriter = XmlWriter.Create(stream, settings))
            {
                xmlSerializer.Serialize(xmlWriter, o);
            }
        }
    }

}