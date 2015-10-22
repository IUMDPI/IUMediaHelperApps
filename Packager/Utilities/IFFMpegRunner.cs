using System.Collections.Generic;
using System.Threading.Tasks;
using Packager.Models.FileModels;
using Packager.Models.PodMetadataModels;

namespace Packager.Utilities
{
    public interface IFFMPEGRunner
    {
        string FFMPEGPath { get; set; }
        Task<ObjectFileModel> CreateDerivative(ObjectFileModel original, ObjectFileModel target, string arguments);
        
        Task Normalize(List<ObjectFileModel> originals);

        Task Normalize(List<ObjectFileModel> originals, ConsolidatedPodMetadata metadata);

        Task Verify(List<ObjectFileModel> originals);

        Task<string> GetFFMPEGVersion();
    }
}