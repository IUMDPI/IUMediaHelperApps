using System.Xml.Serialization;

namespace Common.Models
{
    public class PackagerObjectReport : OperationReport
    {
        [XmlAttribute("Barcode")]
        public string Barcode { get; set; }
    }
}