using System;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;
using Packager.Models;

namespace Packager.Utilities
{
    public interface IProcessRunner
    {
        Task<ProcessResult> Run(ProcessStartInfo startInfo);
    }

    public class ProcessRunner : IProcessRunner
    {
        public Task<ProcessResult> Run(ProcessStartInfo startInfo)
        {
            var completionSource = new TaskCompletionSource<ProcessResult>();

            // just in case
            startInfo.CreateNoWindow = true;
            startInfo.RedirectStandardError = true;
            startInfo.RedirectStandardOutput = true;

            var standardOutput = new StringBuilder();
            var standardError = new StringBuilder();

            var process = new Process
            {
                StartInfo = startInfo,
                EnableRaisingEvents = true
            };

            process.ErrorDataReceived += new DataReceivedHandler(standardError).OnDataReceived;
            process.OutputDataReceived += new DataReceivedHandler(standardOutput).OnDataReceived;

            process.Exited += new ProcessExitHandler(
                completionSource,
                standardOutput,
                standardError).OnExitHandler;

            if (process.Start() == false)
            {
                completionSource.TrySetException(new InvalidOperationException("Failed to start process"));
            }

            process.BeginOutputReadLine();
            process.BeginErrorReadLine();

            return completionSource.Task;
        }

        private class ProcessExitHandler
        {
            private readonly TaskCompletionSource<ProcessResult> _completionSource;
            private readonly StringBuilder _standardErrorBuffer;
            private readonly StringBuilder _standardOutputBuffer;

            public ProcessExitHandler(TaskCompletionSource<ProcessResult> completionSource,
                StringBuilder standardOutputBuffer, StringBuilder standardErrorBuffer)
            {
                _completionSource = completionSource;
                _standardOutputBuffer = standardOutputBuffer;
                _standardErrorBuffer = standardErrorBuffer;
            }

            public void OnExitHandler(object sender, EventArgs args)
            {
                var process = sender as Process;
                if (process == null)
                {
                    _completionSource.TrySetException(new InvalidOperationException("Could not access completed process"));
                    return;
                }

                _completionSource.SetResult(new ProcessResult
                {
                    ExitCode = process.ExitCode,
                    StandardOutput = _standardOutputBuffer.ToString(),
                    StandardError = _standardErrorBuffer.ToString()
                });
            }
        }

        private class DataReceivedHandler
        {
            private readonly StringBuilder _buffer;

            public DataReceivedHandler(StringBuilder buffer)
            {
                _buffer = buffer;
            }

            public void OnDataReceived(object sender, DataReceivedEventArgs args)
            {
                if (string.IsNullOrWhiteSpace(args.Data))
                {
                    return;
                }

                _buffer.AppendLine(args.Data);
            }
        }
    }
}