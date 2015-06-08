using System.Xml.Serialization;

namespace Packager.Models.BextModels
{
    [XmlType(AnonymousType = true)]
    public class ConformancePointDocumentFile
    {
        [XmlAttribute(AttributeName = "name")]
        public string Name { get; set; }

        public ConformancePointDocumentFileCore Core { get; set; }
    }
}