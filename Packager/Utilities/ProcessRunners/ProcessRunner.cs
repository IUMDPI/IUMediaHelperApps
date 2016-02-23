using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Packager.Extensions;
using Packager.Models.ResultModels;

namespace Packager.Utilities.ProcessRunners
{
    public class ProcessRunner : IProcessRunner
    {
        public Task<IProcessResult> Run(ProcessStartInfo startInfo, CancellationToken cancellationToken,
            IOutputBuffer outputBuffer = null, IOutputBuffer errorBuffer = null)
        {
            startInfo.UseShellExecute = false;
         
            var completionSource = new TaskCompletionSource<IProcessResult>();

            var process = new Process
            {
                StartInfo = startInfo,
                EnableRaisingEvents = true
            };

            if (outputBuffer == null)
            {
                outputBuffer = new StringOutputBuffer();
            }

            if (errorBuffer == null)
            {
                errorBuffer = new StringOutputBuffer();
            }

            ConfigureEventHandlers(process, startInfo, outputBuffer, errorBuffer, completionSource);

            if (cancellationToken.IsCancellationRequested)
            {
                cancellationToken.ThrowIfCancellationRequested();
            }

            if (process.Start() == false)
            {
                completionSource.TrySetException(new InvalidOperationException("Failed to start process"));
            }

            cancellationToken.Register(() =>
            {
                DoCancel(process);
                cancellationToken.ThrowIfCancellationRequested();
            });
            
            BeginCaptureOutput(process, startInfo);

            return completionSource.Task;
        }

        private static void DoCancel(Process process)
        {
            if (process.HasExited)
            {
                return;
            }

            process.Kill();
        }
        
        private static void ConfigureEventHandlers(Process process, ProcessStartInfo startInfo, 
            IOutputBuffer outputBuffer, IOutputBuffer errorBuffer, TaskCompletionSource<IProcessResult> completionSource)
        {
            if (startInfo.RedirectStandardOutput)
            {
                process.OutputDataReceived += new DataReceivedHandler(outputBuffer).OnDataReceived;
            }

            if (startInfo.RedirectStandardError)
            {
                process.ErrorDataReceived += new DataReceivedHandler(errorBuffer).OnDataReceived;
            }

            process.Exited += new ProcessExitHandler(
                startInfo,
                completionSource,
                outputBuffer,
                errorBuffer).OnExitHandler;
        }

        private static void BeginCaptureOutput(Process process, ProcessStartInfo startInfo)
        {
            if (startInfo.RedirectStandardOutput)
            {
                process.BeginOutputReadLine();
            }

            if (startInfo.RedirectStandardError)
            {
                process.BeginErrorReadLine();
            }
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
                var process = sender as Process;
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