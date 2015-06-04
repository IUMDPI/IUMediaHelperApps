using System.Diagnostics;

namespace Packager.Models.ProcessResults
{
    public interface IProcessResult
    {
        int ExitCode { get; set; }
        string StandardOutput { get; set; }
        string StandardError { get; set; }
        ProcessStartInfo StartInfo { get; set; }
        bool Succeeded();
    }
}