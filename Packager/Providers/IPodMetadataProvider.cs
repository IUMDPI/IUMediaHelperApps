using System.Collections.Generic;
using System.Threading.Tasks;
using Packager.Models.FileModels;
using Packager.Models.PodMetadataModels;

namespace Packager.Providers
{
    public interface IPodMetadataProvider
    {
        Task<ConsolidatedPodMetadata> GetObjectMetadata(string barcode);
        Task<string> ResolveUnit(string unit);

        void Validate(ConsolidatedPodMetadata podMetadata, List<ObjectFileModel> models);
        void Log(ConsolidatedPodMetadata podMetadata);
    }
}