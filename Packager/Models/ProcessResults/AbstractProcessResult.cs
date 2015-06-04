using System.Diagnostics;

namespace Packager.Models.ProcessResults
{
    public abstract class AbstractProcessResult : IProcessResult
    {
        public int ExitCode { get; set; }
        public string StandardOutput { get; set; }
        public string StandardError { get; set; }
        public ProcessStartInfo StartInfo { get; set; }

        public virtual bool Succeeded()
        {
            return ExitCode == 0;
        }
    }
}