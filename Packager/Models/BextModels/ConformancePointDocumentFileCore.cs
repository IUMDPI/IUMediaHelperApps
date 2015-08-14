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

        #region "serializer control properties"

        // these properties control whether or not the serialize will write
        // empty elements in the final xml.
        // here, it is important to suppress empty elements, because they
        // currently will crash bwfmetaedit.exe

        [XmlIgnore]
        public bool DescriptionSpecified
        {
            get { return !string.IsNullOrWhiteSpace(Description); }
            set { } // serializer requires a setter
        }

        [XmlIgnore]
        public bool OriginatorSpecified
        {
            get { return !string.IsNullOrWhiteSpace(Originator); }
            set { } // serializer requires a setter
        }

        [XmlIgnore]
        public bool OriginatorReferenceSpecified
        {
            get { return !string.IsNullOrWhiteSpace(OriginatorReference); }
            set { } // serializer requires a setter
        }

        [XmlIgnore]
        public bool OriginationDateSpecified
        {
            get { return !string.IsNullOrWhiteSpace(OriginationDate); }
            set { } // serializer requires a setter
        }


        [XmlIgnore]
        public bool OriginationTimeSpecified
        {
            get { return !string.IsNullOrWhiteSpace(OriginationTime); }
            set { } // serializer requires a setter
        }

        [XmlIgnore]
        public bool TimeReferenceTranslatedSpecified
        {
            get { return !string.IsNullOrWhiteSpace(TimeReferenceTranslated); }
            set { } // serializer requires a setter
        }

        [XmlIgnore]
        public bool TimeReferenceSpecified
        {
            get { return !string.IsNullOrWhiteSpace(TimeReference); }
            set { } // serializer requires a setter
        }


        [XmlIgnore]
        public bool BextVersionSpecified
        {
            get { return !string.IsNullOrWhiteSpace(BextVersion); }
            set { } // serializer requires a setter
        }

        [XmlIgnore]
        public bool UMIDSpecified
        {
            get { return !string.IsNullOrWhiteSpace(UMID); }
            set { } // serializer requires a setter
        }

        [XmlIgnore]
        public bool CodingHistorySpecified
        {
            get { return !string.IsNullOrWhiteSpace(CodingHistory); }
            set { } // serializer requires a setter
        }

        [XmlIgnore]
        public bool IARLSpecified
        {
            get { return !string.IsNullOrWhiteSpace(IARL); }
            set { } // serializer requires a setter
        }

        [XmlIgnore]
        public bool IARTSpecified
        {
            get { return !string.IsNullOrWhiteSpace(IART); }
            set { } // serializer requires a setter
        }

        [XmlIgnore]
        public bool ICMSSpecified
        {
            get { return !string.IsNullOrWhiteSpace(ICMS); }
            set { } // serializer requires a setter
        }

        [XmlIgnore]
        public bool ICMTSpecified
        {
            get { return !string.IsNullOrWhiteSpace(ICMT); }
            set { } // serializer requires a setter
        }

        [XmlIgnore]
        public bool ICOPSpecified
        {
            get { return !string.IsNullOrWhiteSpace(ICOP); }
            set { } // serializer requires a setter
        }

        [XmlIgnore]
        public bool ICRDSpecified
        {
            get { return !string.IsNullOrWhiteSpace(ICRD); }
            set { } // serializer requires a setter
        }

        [XmlIgnore]
        public bool IGNRSpecified
        {
            get { return !string.IsNullOrWhiteSpace(IGNR); }
            set { } // serializer requires a setter
        }

        [XmlIgnore]
        public bool INAMSpecified
        {
            get { return !string.IsNullOrWhiteSpace(INAM); }
            set { } // serializer requires a setter
        }

        [XmlIgnore]
        public bool IPRDSpecified
        {
            get { return !string.IsNullOrWhiteSpace(IPRD); }
            set { } // serializer requires a setter
        }

        [XmlIgnore]
        public bool ISBJSpecified
        {
            get { return !string.IsNullOrWhiteSpace(ISBJ); }
            set { } // serializer requires a setter
        }

        [XmlIgnore]
        public bool IKEYSpecified
        {
            get { return !string.IsNullOrWhiteSpace(IKEY); }
            set { } // serializer requires a setter
        }

        [XmlIgnore]
        public bool ISFTSpecified
        {
            get { return !string.IsNullOrWhiteSpace(ISFT); }
            set { } // serializer requires a setter
        }

        [XmlIgnore]
        public bool ISRCSpecified
        {
            get { return !string.IsNullOrWhiteSpace(ISRC); }
            set { } // serializer requires a setter
        }

        [XmlIgnore]
        public bool ITCHSpecified
        {
            get { return !string.IsNullOrWhiteSpace(ITCH); }
            set { } // serializer requires a setter
        }

        [XmlIgnore]
        public bool IENGSpecified
        {
            get { return !string.IsNullOrWhiteSpace(IENG); }
            set { } // serializer requires a setter
        }

        [XmlIgnore]
        public bool IMEDSpecified
        {
            get { return !string.IsNullOrWhiteSpace(IMED); }
            set { } // serializer requires a setter
        }


        [XmlIgnore]
        public bool ISRFSpecified
        {
            get { return !string.IsNullOrWhiteSpace(ISRF); }
            set { } // serializer requires a setter
        }

        #endregion
    }
}