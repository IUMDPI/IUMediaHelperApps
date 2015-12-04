using System.Threading.Tasks;
using Packager.Models.FileModels;

namespace Packager.Utilities.Process
{
    public interface IFFProbeRunner
    {
        string FFProbePath { get; }
        string VideoQualityControlArguments { get; }
        Task<QualityControlFileModel> GenerateQualityControlFile(ObjectFileModel target);

        Task<string> GetVersion();
    }
}