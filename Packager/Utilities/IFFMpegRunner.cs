using System.Collections.Generic;
using System.Threading.Tasks;
using Packager.Models.FileModels;

namespace Packager.Utilities
{
    public interface IFFMPEGRunner
    {
        string FFMPEGPath { get; set; }
        Task<ObjectFileModel> CreateDerivative(ObjectFileModel original, ObjectFileModel target, string arguments);
        
        Task Normalize(List<ObjectFileModel> originals);

        Task Verify(List<ObjectFileModel> originals);

        Task<string> GetFFMPEGVersion();
    }
}