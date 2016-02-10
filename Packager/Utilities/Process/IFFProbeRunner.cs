using System.Threading.Tasks;
using Packager.Models.FileModels;

namespace Packager.Utilities.Process
{
    public interface IFFProbeRunner
    {
        string FFProbePath { get; }
        string VideoQualityControlArguments { get; }
        Task<QualityControlFile> GenerateQualityControlFile(AbstractFile target);

        Task<string> GetVersion();
    }
}