using System.Xml.Serialization;
using Packager.Attributes;
using Packager.Utilities;

namespace Packager.Models.BextModels
{
    [XmlType(AnonymousType = true)]
    public class ConformancePointDocumentFileCore
    {
        [BextField(BextFields.Description)]
        public string Description { get; set; }

        [BextField(BextFields.Originator)]
        public string Originator { get; set; }

        [BextField(BextFields.OriginatorReference)]
        public string OriginatorReference { get; set; }

        [BextField(BextFields.OriginationDate)]
        public string OriginationDate { get; set; }

        [BextField(BextFields.OriginationTime)]
        public string OriginationTime { get; set; }

        [XmlElement("TimeReference_translated")]
        public string TimeReferenceTranslated { get; set; }

        [BextField(BextFields.TimeReference)]
        public string TimeReference { get; set; }

        public string BextVersion { get; set; }

        [BextField(BextFields.UMID)]
        public string UMID { get; set; }

        [BextField(BextFields.History)]
        public string CodingHistory { get; set; }

        [BextField(BextFields.IARL)]
        public string IARL { get; set; }

        [BextField(BextFields.IART)]
        public string IART { get; set; }

        [BextField(BextFields.ICMS)]
        public string ICMS { get; set; }

        [BextField(BextFields.ICMT)]
        public string ICMT { get; set; }

        [BextField(BextFields.ICOP)]
        public string ICOP { get; set; }

        [BextField(BextFields.ICRD)]
        public string ICRD { get; set; }

        [BextField(BextFields.IENG)]
        public string IENG { get; set; }

        [BextField(BextFields.IGNR)]
        public string IGNR { get; set; }

        [BextField(BextFields.IKEY)]
        public string IKEY { get; set; }

        [BextField(BextFields.IMED)]
        public string IMED { get; set; }

        [BextField(BextFields.INAM)]
        public string INAM { get; set; }

        [BextField(BextFields.IPRD)]
        public string IPRD { get; set; }

        [BextField(BextFields.ISBJ)]
        public string ISBJ { get; set; }

        [BextField(BextFields.ISFT)]
        public string ISFT { get; set; }

        [BextField(BextFields.ISRC)]
        public string ISRC { get; set; }

        [BextField(BextFields.ISRF)]
        public string ISRF { get; set; }

        [BextField(BextFields.ITCH)]
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