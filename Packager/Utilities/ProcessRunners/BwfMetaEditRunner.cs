using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Packager.Extensions;
using Packager.Models.FileModels;
using Packager.Models.ResultModels;
using Packager.Utilities.Bext;
using Packager.Validators.Attributes;

namespace Packager.Utilities.ProcessRunners
{
    public class BwfMetaEditRunner : IBwfMetaEditRunner
    {
        private const string VerboseArgument = "--verbose";
        private const string VersionArgument = "--version";

        public BwfMetaEditRunner(IProcessRunner processRunner, string bwfMetaEditPath, string baseProcessingDirectory)
        {
            ProcessRunner = processRunner;
            BwfMetaEditPath = bwfMetaEditPath;
            BaseProcessingDirectory = baseProcessingDirectory;
        }

        private IProcessRunner ProcessRunner { get; }
        private string BaseProcessingDirectory { get; }
     
        [ValidateFile]
        public string BwfMetaEditPath { get; }

        public async Task<IProcessResult> ClearMetadata(AbstractFile model, IEnumerable<BextFields> fields, CancellationToken cancellationToken)
        {
            var arguments = new ArgumentBuilder(VerboseArgument);
        
            foreach (var field in fields)
            {
                arguments.AddArguments($"--{field}=\"\"");
            }

            arguments.AddArguments(Path.Combine(BaseProcessingDirectory, model.GetFolderName(), model.Filename).ToQuoted());
            return await ExecuteBextProcess(arguments, cancellationToken);
        }

        public async Task<string> GetVersion()
        {
            try
            {
                var result = await ExecuteBextProcess(new ArgumentBuilder(VersionArgument), CancellationToken.None);

                var parts = result.StandardOutput.GetContent().Split(new[] {"\r\n"}, StringSplitOptions.RemoveEmptyEntries);
                return parts.Last();
            }
            catch (Exception)
            {
                return "";
            }
        }

        private async Task<IProcessResult> ExecuteBextProcess(IEnumerable arguments, CancellationToken cancellationToken)
        {
            var startInfo = new ProcessStartInfo(BwfMetaEditPath)
            {
                Arguments = arguments.ToString(),
                RedirectStandardError = true,
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true,
            };

            return await ProcessRunner.Run(startInfo, cancellationToken);
        }
    }
}