using System.Collections.Generic;
using System.Threading.Tasks;
using Packager.Models.EmbeddedMetadataModels;
using Packager.Models.FileModels;

namespace Packager.Utilities
{
    public interface IFFMPEGRunner
    {
        string FFMPEGPath { get; set; }

        string ProdOrMezzArguments { get; }
        string AccessArguments { get; }

        Task<string> GetFFMPEGVersion();

        Task Normalize(ObjectFileModel original, AbstractEmbeddedMetadata metadata);

        Task Verify(List<ObjectFileModel> originals);

        Task<ObjectFileModel> CreateProdOrMezzDerivative(ObjectFileModel original, ObjectFileModel target, AbstractEmbeddedMetadata metadata);

        Task<ObjectFileModel> CreateAccessDerivative(ObjectFileModel original);
    }
}