using System.Collections.Generic;
using System.Threading.Tasks;
using Packager.Models.EmbeddedMetadataModels;
using Packager.Models.FileModels;
using Packager.Models.PodMetadataModels;

namespace Packager.Utilities
{
    public interface IFFMPEGRunner
    {
        string FFMPEGPath { get; set; }
        
        Task<string> GetFFMPEGVersion();

        Task Normalize(ObjectFileModel original, AbstractEmbeddedMetadata metadata);

        Task Verify(List<ObjectFileModel> originals);

        Task<ObjectFileModel> CreateProdOrMezzDerivative(ObjectFileModel original, ObjectFileModel target, AbstractEmbeddedMetadata metadata);

        Task<ObjectFileModel> CreateAccessDerivative(ObjectFileModel original);

        string ProdOrMezzArguments { get; }
        string AccessArguments { get; }

    }
}