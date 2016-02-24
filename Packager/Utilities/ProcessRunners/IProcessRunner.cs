using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Packager.Models.ResultModels;

namespace Packager.Utilities.ProcessRunners
{
    public interface IProcessRunner
    {
        Task<IProcessResult> Run(
            ProcessStartInfo startInfo,
            CancellationToken cancellationToken,
            IOutputBuffer outputBuffer = null,
            IOutputBuffer errorBuffer = null);
    }
}