using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Packager.Attributes;
using Packager.Extensions;
using Packager.Models.BextModels;
using Packager.Models.FileModels;
using Packager.Models.ResultModels;
using Packager.Validators.Attributes;

namespace Packager.Utilities
{
    public interface IBwfMetaEditRunner
    {
        Task<IProcessResult> AddMetadata(ObjectFileModel model, ConformancePointDocumentFileCore core);
        Task<IProcessResult> ClearMetadata(ObjectFileModel model);
        Task<string> GetVersion();

        string BwfMetaEditPath { get; }
    }

    public class BwfMetaEditRunner : IBwfMetaEditRunner
    {
        private const string VerboseArgument = "--verbose";
        private const string AppendArgument = "--append";
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

        public async Task<IProcessResult> AddMetadata(ObjectFileModel model, ConformancePointDocumentFileCore core)
        {
            var args = GetArgsForCoreAndModel(model, core);
            return await ExecuteBextProcess(args);
        }

        public async Task<IProcessResult> ClearMetadata(ObjectFileModel model)
        {
            var args = GetArgsForCoreAndModel(model, new ConformancePointDocumentFileCore());
            return await ExecuteBextProcess(args);
        }

        public async Task<string> GetVersion()
        {
            try
            {
                var result = await ExecuteBextProcess(VersionArgument);

                var parts = result.StandardOutput.Split(new[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
                return parts.Last();
            }
            catch (Exception)
            {
                return "";
            }
        }

        private string GetArgsForCoreAndModel(AbstractFileModel model, ConformancePointDocumentFileCore core)
        {
            var args = new List<string> {VerboseArgument, AppendArgument};
            args.AddRange(core.GetType().GetProperties()
                .Select(p => new Tuple<PropertyInfo, BextFieldAttribute>(p, p.GetCustomAttribute<BextFieldAttribute>()))
                .Where(t => t.Item2 != null)
                .Select(t => $"--{t.Item2.Field}={t.Item1.GetValue(core).ToDefaultIfEmpty().ToQuoted()}"));

            args.Add(Path.Combine(BaseProcessingDirectory, model.GetFolderName(), model.ToFileName()).ToQuoted());

            return string.Join(" ", args);
        }


        private async Task<IProcessResult> ExecuteBextProcess(string args)
        {
            var startInfo = new ProcessStartInfo(BwfMetaEditPath)
            {
                Arguments = args,
                RedirectStandardError = true,
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            return await ProcessRunner.Run(startInfo);
        }
    }
}