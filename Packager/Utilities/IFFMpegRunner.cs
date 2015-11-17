using System.Collections.Generic;
using System.Threading.Tasks;
using Packager.Models.BextModels;
using Packager.Models.FileModels;
using Packager.Models.PodMetadataModels;

namespace Packager.Utilities
{
    public interface IAudioFFMPEGRunner:IFFMPEGRunner
    {
        Task<ObjectFileModel> CreateProductionDerivative(ObjectFileModel original, ObjectFileModel target, BextMetadata metadata);

        Task<ObjectFileModel> CreateAccessDerivative(ObjectFileModel original);

        Task Normalize(ObjectFileModel original, BextMetadata metadata);

        Task Verify(List<ObjectFileModel> originals);

        string ProductionArguments { get; }
        string AccessArguments { get; }

    }

    public interface IVideoFFMPEGRunner : IFFMPEGRunner
    {
        Task<ObjectFileModel> CreateMezzanineDerivative(ObjectFileModel original, ObjectFileModel target, VideoPodMetadata metadata);

        Task<ObjectFileModel> CreateAccessDerivative(ObjectFileModel original);

        Task Normalize(ObjectFileModel original, BextMetadata metadata);

        Task Verify(List<ObjectFileModel> originals);

        string MezzanineArguments { get; }
        string AccessArguments { get; }

    }


    public interface IFFMPEGRunner
    {
        string FFMPEGPath { get; set; }
        
        Task<string> GetFFMPEGVersion();
    }
}