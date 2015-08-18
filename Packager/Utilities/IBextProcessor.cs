using System.Collections.Generic;
using System.Threading.Tasks;
using Packager.Models.FileModels;
using Packager.Models.PodMetadataModels;

namespace Packager.Utilities
{
    public interface IBextProcessor
    {
        Task EmbedBextMetadata(List<ObjectFileModel> instances, ConsolidatedPodMetadata podMetadata);
        Task<string> GetBwfMetaEditVersion();
        string BwfMetaEditPath { get; set; }
    }
}