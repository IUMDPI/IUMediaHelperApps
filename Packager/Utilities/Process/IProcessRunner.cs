using System.Diagnostics;
using System.Threading.Tasks;
using Packager.Models.ResultModels;

namespace Packager.Utilities.Process
{
    public interface IProcessRunner
    {
        Task<IProcessResult> Run(ProcessStartInfo startInfo, IOutputBuffer outputBuffer = null, IOutputBuffer errorBuffer = null);

    }
}