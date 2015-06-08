using System.Xml.Serialization;

namespace Packager.Models.BextModels
{
    
    [XmlType(AnonymousType = true)]
    [XmlRoot(Namespace = "", IsNullable = false, ElementName = "conformance_point_document")]
    public class ConformancePointDocument
    {
        [XmlElement("File")]
        public ConformancePointDocumentFile[] File { get; set; }
    }
}