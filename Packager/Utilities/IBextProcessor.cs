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
       
        Task EmbedBextMetadata(List<ObjectFileModel> instances, ConsolidatedPodMetadata podMetadata);

        Task ClearAllBextMetadataFields(List<ObjectFileModel> instances);
        
    }
}