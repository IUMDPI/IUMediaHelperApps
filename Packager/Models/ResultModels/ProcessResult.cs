using System.Diagnostics;
using Packager.Utilities.Process;

namespace Packager.Models.ResultModels
{
    public class ProcessResult : IProcessResult
    {
        public int ExitCode { get; set; }
        public IOutputBuffer StandardOutput { get; set; }
        public IOutputBuffer StandardError { get; set; }
        public ProcessStartInfo StartInfo { get; set; }
    }
}