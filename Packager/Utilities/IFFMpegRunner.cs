using System.Collections.Generic;
using System.Threading.Tasks;
using Packager.Models.EmbeddedMetadataModels;
using Packager.Models.FileModels;
using Packager.Models.PodMetadataModels;

namespace Packager.Utilities
{
    public interface IAudioFFMPEGRunner:IFFMPEGRunner
    {
        Task<ObjectFileModel> CreateProductionDerivative(ObjectFileModel original, ObjectFileModel target, EmbeddedAudioMetadata metadata);

        Task<ObjectFileModel> CreateAccessDerivative(ObjectFileModel original);

        Task Normalize(ObjectFileModel original, EmbeddedAudioMetadata metadata);

        Task Verify(List<ObjectFileModel> originals);

        string ProductionArguments { get; }
        string AccessArguments { get; }

    }

    public interface IVideoFFMPEGRunner : IFFMPEGRunner
    {
        Task<ObjectFileModel> CreateMezzanineDerivative(ObjectFileModel original, ObjectFileModel target);

        Task<ObjectFileModel> CreateAccessDerivative(ObjectFileModel original);

        Task EmbedMetadata(List<ObjectFileModel> fileModels, VideoPodMetadata metadata);

        string MezzanineArguments { get; }
        string AccessArguments { get; }

    }


    public interface IFFMPEGRunner
    {
        string FFMPEGPath { get; set; }
        
        Task<string> GetFFMPEGVersion();
    }
}