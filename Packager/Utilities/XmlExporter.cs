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
            var xmlSerializer = new XmlSerializer(o.GetType());

            using (Stream stream = new FileStream(path, FileMode.Create))
            using (XmlWriter xmlWriter = new XmlTextWriter(stream, encoding))
            {
                xmlSerializer.Serialize(xmlWriter, o);
            }
        }
    }

}