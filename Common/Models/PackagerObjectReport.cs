using System.Xml.Serialization;

namespace Common.Models
{
    public class PackagerObjectReport : AbstractOperationReport
    {
        [XmlAttribute("Barcode")]
        public string Barcode { get; set; }
    }
}