using System.Collections.Generic;
using System.Xml.Serialization;

namespace Common.Models
{
    public class PackagerReport : OperationReport
    {
        [XmlArray("Objects")]
        [XmlArrayItem("Object")]
        public List<PackagerObjectReport> ObjectReports { get; set; }
    }
}
