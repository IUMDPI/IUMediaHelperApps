using System.Collections.Generic;
using System.Threading.Tasks;
using Packager.Models.FileModels;
using Packager.Models.PodMetadataModels;

namespace Packager.Utilities
{
    public enum BextFields
    {
        Description,
        Originator,
        OriginatorReference,
        OriginationDate,
        OriginationTime,
        TimeReference,
        History,
        UMID,
        IARL,
        IART,
        ICMS,
        ICMT,
        ICOP,
        ICRD,
        IENG,
        IGNR,
        IKEY,
        IMED,
        INAM,
        IPRD,
        ISBJ,
        ISFT,
        ISRC,
        ISRF,
        ITCH
    }

    public interface IBextProcessor
    {
        string BwfMetaEditPath { get; set; }
        Task EmbedBextMetadata(List<ObjectFileModel> instances, ConsolidatedPodMetadata podMetadata);

        Task ClearBextMetadataFields(List<ObjectFileModel> instances, IEnumerable<BextFields> fields);

        Task ClearAllBextMetadataFields(List<ObjectFileModel> instances);

        Task<string> GetBwfMetaEditVersion();
    }
}