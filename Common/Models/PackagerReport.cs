using System.Collections.Generic;
using System.Xml.Serialization;

namespace Common.Models
{
    public class PackagerReport : AbstractOperationReport
    {
        [XmlArray("Objects")]
        [XmlArrayItem("Object")]
        public List<PackagerObjectReport> ObjectReports { get; set; }
    }
}
