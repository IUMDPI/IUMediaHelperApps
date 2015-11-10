using System.Collections.Generic;
using System.Threading.Tasks;
using Packager.Models.FileModels;
using Packager.Models.PodMetadataModels.ConsolidatedModels;

namespace Packager.Providers
{
    public interface IPodMetadataProvider
    {
        Task<T> GetObjectMetadata<T>(string barcode) where T : AbstractConsolidatedPodMetadata, new();

        Task<string> ResolveUnit(string unit);

        void Validate<T>(T podMetadata, List<ObjectFileModel> models) where T : AbstractConsolidatedPodMetadata;

        void Log<T>(T podMetadata) where T : AbstractConsolidatedPodMetadata;
    }
}