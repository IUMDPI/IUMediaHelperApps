using System.Diagnostics;
using System.Threading.Tasks;
using Packager.Models.ResultModels;

namespace Packager.Utilities
{
    public interface IProcessRunner
    {
        Task<IProcessResult> Run(ProcessStartInfo startInfo);
    }
}