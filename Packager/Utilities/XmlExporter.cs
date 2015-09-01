using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using Packager.Annotations;

namespace Packager.Utilities
{
    internal class XmlExporter : IXmlExporter
    {
        public void ExportToFile(object o, string path, Encoding encoding)
        {
            var xmlSerializer = new XmlSerializer(o.GetType());

            using (Stream stream = new FileStream(path, FileMode.Create))
            using (var xmlWriter = new EntitizingXmlWriter(stream, encoding) {Formatting = Formatting.Indented})
            {
                xmlSerializer.Serialize(xmlWriter, o);
            }
        }
    }

    // adapted from http://stackoverflow.com/questions/8811873/xdocument-save-removes-my-xa-entities
    // we need to convert \r\n to &#xD;&#xA;
    public class EntitizingXmlWriter : XmlTextWriter
    {
        public EntitizingXmlWriter([NotNull] Stream w, Encoding encoding) : base(w, encoding)
        {
        }

        public override void WriteString(string text)
        {
            // The start index of the next substring containing only non-entitized characters.
            var start = 0;

            // The index of the current character being checked.
            for (var curr = 0; curr < text.Length; ++curr)
            {
                // Check whether the current character should be entitized.
                var chr = text[curr];

                // if not, continue
                if (chr != '\r' && chr != '\n' && chr != '\t')
                {
                    continue;
                }

                // Write the previous substring of non-entitized characters.
                if (start < curr)
                    base.WriteString(text.Substring(start, curr - start));

                // Write current character, entitized.
                WriteCharEntity(chr);

                // Next substring of non-entitized characters tentatively starts
                // immediately beyond current character.
                start = curr + 1;
            }

            // Write the trailing substring of non-entitized characters.
            if (start < text.Length)
                base.WriteString(text.Substring(start, text.Length - start));
        }
    }
}