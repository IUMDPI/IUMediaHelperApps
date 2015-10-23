using System.Collections.Generic;
using System.Threading.Tasks;
using Packager.Models.BextModels;
using Packager.Models.FileModels;

namespace Packager.Utilities
{
    public interface IFFMPEGRunner
    {
        string FFMPEGPath { get; set; }

        Task<ObjectFileModel> CreateProductionDerivative(ObjectFileModel original, BextMetadata metadata);

        Task<ObjectFileModel> CreateAccessDerivative(ObjectFileModel original);
        
        Task Normalize(ObjectFileModel original, BextMetadata metadata);

        Task Verify(List<ObjectFileModel> originals);

        Task<string> GetFFMPEGVersion();
    }
}