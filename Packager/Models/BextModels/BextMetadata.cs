using System.Xml.Serialization;
using Packager.Attributes;
using Packager.Utilities;

namespace Packager.Models.BextModels
{
   
    public class BextMetadata
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

        [BextField(BextFields.TimeReference)]
        public string TimeReference { get; set; }

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

    }
}