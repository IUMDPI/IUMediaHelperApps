using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Packager.Extensions;
using Packager.Models.FileModels;
using Packager.Models.ResultModels;
using Packager.Validators.Attributes;

namespace Packager.Utilities
{
    public interface IBwfMetaEditRunner
    {
        string BwfMetaEditPath { get; }
        //Task<IProcessResult> AddMetadata(ObjectFileModel model, BextMetadata core);
        //Task<IProcessResult> ClearMetadata(ObjectFileModel model);

        Task<IProcessResult> ClearMetadata(ObjectFileModel model, IEnumerable<BextFields> fields);
        Task<string> GetVersion();
    }

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

        public async Task<IProcessResult> ClearMetadata(ObjectFileModel model, IEnumerable<BextFields> fields)
        {
            var arguments = new ArgumentBuilder(VerboseArgument);
        
            foreach (var field in fields)
            {
                arguments.AddArguments($"--{field}=\"\"");
            }

            arguments.AddArguments(Path.Combine(BaseProcessingDirectory, model.GetFolderName(), model.ToFileName()).ToQuoted());
            return await ExecuteBextProcess(arguments);
        }

        public async Task<string> GetVersion()
        {
            try
            {
                var result = await ExecuteBextProcess(new ArgumentBuilder(VersionArgument));

                var parts = result.StandardOutput.Split(new[] {"\r\n"}, StringSplitOptions.RemoveEmptyEntries);
                return parts.Last();
            }
            catch (Exception)
            {
                return "";
            }
        }

        private async Task<IProcessResult> ExecuteBextProcess(ArgumentBuilder arguments)
        {
            var startInfo = new ProcessStartInfo(BwfMetaEditPath)
            {
                Arguments = arguments.ToString(),
                RedirectStandardError = true,
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            return await ProcessRunner.Run(startInfo);
        }
    }
}