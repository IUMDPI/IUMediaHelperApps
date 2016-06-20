using System.Threading;
using System.Threading.Tasks;
using Packager.Models.FileModels;

namespace Packager.Utilities.ProcessRunners
{
    public interface IFFProbeRunner
    {
        string FFProbePath { get; }
        string VideoQualityControlArguments { get; }
        Task<QualityControlFile> GenerateQualityControlFile(AbstractFile target, CancellationToken cancellationToken);

        Task<string> GetVersion();

        Task<InfoFile> GetMediaInfo(AbstractFile target, CancellationToken cancellation);
    }
}