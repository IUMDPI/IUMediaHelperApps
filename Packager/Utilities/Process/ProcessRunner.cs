using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Packager.Extensions;
using Packager.Models.ResultModels;

namespace Packager.Utilities.Process
{
    public class ProcessRunner : IProcessRunner
    {
        public Task<IProcessResult> Run(ProcessStartInfo startInfo, IOutputBuffer outputBuffer =null, IOutputBuffer errorBuffer = null)
        {
            var completionSource = new TaskCompletionSource<IProcessResult>();

            // just in case
            startInfo.CreateNoWindow = true;
            startInfo.RedirectStandardError = true;
            startInfo.RedirectStandardOutput = true;
            startInfo.UseShellExecute = false;
            
            var process = new System.Diagnostics.Process
            {
                StartInfo = startInfo,
                EnableRaisingEvents = true,
            };

            if (outputBuffer == null)
            {
                outputBuffer = new StringOutputBuffer();
            }

            if (errorBuffer == null)
            {
                errorBuffer = new StringOutputBuffer();
            }

            process.ErrorDataReceived += new DataReceivedHandler(errorBuffer).OnDataReceived;
            process.OutputDataReceived += new DataReceivedHandler(outputBuffer).OnDataReceived;

            process.Exited += new ProcessExitHandler(
                startInfo,
                completionSource,
                outputBuffer,
                errorBuffer).OnExitHandler;

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
            private readonly ProcessStartInfo _startInfo;
            private readonly TaskCompletionSource<IProcessResult> _completionSource;
            private readonly IOutputBuffer _errorBuffer;
            private readonly IOutputBuffer _outputBuffer;

            public ProcessExitHandler(ProcessStartInfo startInfo, TaskCompletionSource<IProcessResult> completionSource,
                IOutputBuffer outputBuffer, IOutputBuffer errorBuffer)
            {
                _startInfo = startInfo;
                _completionSource = completionSource;
                _outputBuffer = outputBuffer;
                _errorBuffer = errorBuffer;
            }

            public void OnExitHandler(object sender, EventArgs args)
            {
                var process = sender as System.Diagnostics.Process;
                if (process == null)
                {
                    _completionSource.TrySetException(new InvalidOperationException("Could not access completed process"));
                    return;
                }

                // need to do this to ensure that all
                // event handlers have completed, etc.
                process.WaitForExit();

                _completionSource.SetResult(new ProcessResult
                {
                    StartInfo = _startInfo,
                    ExitCode = process.ExitCode,
                    StandardOutput = _outputBuffer,
                    StandardError = _errorBuffer
                });
            }
        }

        private class DataReceivedHandler
        {
            private readonly IOutputBuffer _buffer;

            public DataReceivedHandler(IOutputBuffer buffer)
            {
                _buffer = buffer;
            }

            public void OnDataReceived(object sender, DataReceivedEventArgs args)
            {
                if (args.Data.IsNotSet())
                {
                    return;
                }

                _buffer.AppendLine(args.Data);
            }
        }
    }
}