using System.Collections.Generic;
using System.Threading.Tasks;
using Packager.Models.FileModels;
using Packager.Models.PodMetadataModels;

namespace Packager.Providers
{
    public interface IPodMetadataProvider
    {
        Task<T> GetObjectMetadata<T>(string barcode) where T : AbstractPodMetadata, new();
        void Validate<T>(T podMetadata, List<ObjectFileModel> models) where T : AbstractPodMetadata;
        void Log<T>(T podMetadata) where T : AbstractPodMetadata;
    }
}