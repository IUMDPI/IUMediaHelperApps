using System.Threading.Tasks;
using Packager.Models.FileModels;

namespace Packager.Utilities
{
    public interface IFFMPEGRunner
    {
        string FFMPEGPath { get; set; }
        Task<ObjectFileModel> CreateDerivative(ObjectFileModel original, ObjectFileModel target, string arguments);
        Task<string> GetFFMPEGVersion();
    }
}