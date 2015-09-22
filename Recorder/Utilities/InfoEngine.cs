using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Recorder.Models;

namespace Recorder.Utilities
{
    public class InfoEngine
    {
        public InfoEngine(IProgramSettings settings)
        {
            PathToFFProbe = settings.PathToFFProbe;
        }

        private string PathToFFProbe { get; }

        public async Task<TimeSpan> GetDuration(string path)
        {
            var info = new ProcessStartInfo(PathToFFProbe)
            {
                UseShellExecute = false,
                CreateNoWindow = true,
                RedirectStandardError = true,
                RedirectStandardOutput = true,
                Arguments = $"-v quiet -show_entries format=duration -sexagesimal -of default=noprint_wrappers=1:nokey=1 \"{path}\""
            };

            using (var process = new Process {StartInfo = info, EnableRaisingEvents = true})
            {
                return await GetInfoAsync(process).ConfigureAwait(false);
            }
        }


        private static Task<TimeSpan> GetInfoAsync(Process process)
        {
            var completionSource = new TaskCompletionSource<TimeSpan>();

            process.Exited += (s, ea) => completionSource.SetResult(GetOutputAsTimespan(process));
            process.Start();

            return completionSource.Task;
        }

        private static TimeSpan GetOutputAsTimespan(Process process)
        {
            if (process.ExitCode != 0)
            {
                return new TimeSpan();
            }

            TimeSpan result;

            var output = process.StandardOutput.ReadToEnd().Replace("\r\n", "");
            return TimeSpan.TryParse(output, out result)
                ? result
                : new TimeSpan();
        }
    }
}