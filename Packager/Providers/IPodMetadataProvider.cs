using System.CodeDom;
using System.Collections.Generic;
using System.Threading.Tasks;
using Packager.Models.FileModels;
using Packager.Models.PodMetadataModels;

namespace Packager.Providers
{
    public interface IPodMetadataProvider
    {
        Task<ConsolidatedPodMetadata> Get(string barcode);
        void Validate(ConsolidatedPodMetadata podMetadata, List<ObjectFileModel> models);
        void Log(ConsolidatedPodMetadata podMetadata);

    }
}