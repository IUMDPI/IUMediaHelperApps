using System.Diagnostics;

namespace Packager.Models.ResultModels
{
    public class ProcessResult : IProcessResult
    {
        public int ExitCode { get; set; }
        public string StandardOutput { get; set; }
        public string StandardError { get; set; }

        public ProcessStartInfo StartInfo { get; set; }
    }
}