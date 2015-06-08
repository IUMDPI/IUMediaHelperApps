using System;
using System.Xml.Serialization;

namespace Packager.Models.BextModels
{
    [XmlType(AnonymousType = true)]
    public class ConformancePointDocumentFileCore
    {
        public string Description { get; set; }
        public string Originator { get; set; }
        public string OriginatorReference { get; set; }
        public string OriginationDate { get; set; }
        public string OriginationTime { get; set; }
        [XmlElement("TimeReference_translated")]
        public string TimeReferenceTranslated { get; set; }
        public string TimeReference { get; set; }
        public string BextVersion { get; set; }
        public string UMID { get; set; }
        public string CodingHistory { get; set; }
        public string IARL { get; set; }
        public string IART { get; set; }
        public string ICMS { get; set; }
        public string ICMT { get; set; }
        public string ICOP { get; set; }
        public string ICRD { get; set; }
        public string IENG { get; set; }
        public string IGNR { get; set; }
        public string IKEY { get; set; }
        public string IMED { get; set; }
        public string INAM { get; set; }
        public string IPRD { get; set; }
        public string ISBJ { get; set; }
        public string ISFT { get; set; }
        public string ISRC { get; set; }
        public string ISRF { get; set; }
        public string ITCH { get; set; }
    }
}