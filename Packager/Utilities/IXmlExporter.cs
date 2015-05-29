using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace Packager.Utilities
{
    interface IXmlExporter
    {
        string GenerateXml(object o);
    }

    class XmlExporter : IXmlExporter
    {
        public string GenerateXml(object o)
        {
            var serializer = new XmlSerializer(o.GetType());
         
            // code borrow
            using (var textWriter = new StringWriter())
            {
                using (XmlWriter xmlWriter = XmlWriter.Create(textWriter))
                {
                    serializer.Serialize(xmlWriter, o);
                }
                return textWriter.ToString();
            }
        }
    }
}
