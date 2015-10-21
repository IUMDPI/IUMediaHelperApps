using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Packager.Attributes;
using Packager.Exceptions;
using Packager.Extensions;
using Packager.Models.BextModels;
using Packager.Models.FileModels;
using Packager.Models.ResultModels;
using Packager.Validators.Attributes;

namespace Packager.Utilities
{
    public interface IBwfMetaEditRunner
    {
        string BwfMetaEditPath { get; }
        Task<IProcessResult> AddMetadata(ObjectFileModel model, BextMetadata core);
        Task<IProcessResult> ClearMetadata(ObjectFileModel model);
        Task<string> GetVersion();
    }

    public class BwfMetaEditRunner : IBwfMetaEditRunner
    {
        private const string VerboseArgument = "--verbose";
        private const string AppendArgument = "--append";
        private const string VersionArgument = "--version";

        public BwfMetaEditRunner(IProcessRunner processRunner, string bwfMetaEditPath, string baseProcessingDirectory, 
            BextFields[] suppressFields, bool useAppend)
        {
            ProcessRunner = processRunner;
            BwfMetaEditPath = bwfMetaEditPath;
            BaseProcessingDirectory = baseProcessingDirectory;
            SuppressFields = suppressFields;
            UseAppend = useAppend;
        }

        private IProcessRunner ProcessRunner { get; }
        private string BaseProcessingDirectory { get; }
        private BextFields[] SuppressFields { get; set; }
        private bool UseAppend { get; set; }

        [ValidateFile]
        public string BwfMetaEditPath { get; }

        public async Task<IProcessResult> AddMetadata(ObjectFileModel model, BextMetadata core)
        {
            var args = GetArgsForCoreAndModel(model, core);
            return await ExecuteBextProcess(args);
        }

        public async Task<IProcessResult> ClearMetadata(ObjectFileModel model)
        {
            var args = GetArgsForCoreAndModel(model, new BextMetadata());
            return await ExecuteBextProcess(args);
        }

        public async Task<string> GetVersion()
        {
            try
            {
                var result = await ExecuteBextProcess(VersionArgument);

                var parts = result.StandardOutput.Split(new[] {"\r\n"}, StringSplitOptions.RemoveEmptyEntries);
                return parts.Last();
            }
            catch (Exception)
            {
                return "";
            }
        }

        private string GetArgsForCoreAndModel(AbstractFileModel model, BextMetadata core)
        {
            var args = new List<string> {VerboseArgument}; //, AppendArgument};

            if (UseAppend)
            {
                args.Add(AppendArgument);
            }

            if (!string.IsNullOrWhiteSpace(core.CodingHistory) && core.CodingHistory.Length%2 != 0)
            {
                core.CodingHistory = core.CodingHistory + " ";
            }

            foreach (var info in core.GetType().GetProperties()
                .Select(p => new Tuple<string, BextFieldAttribute>(GetValueFromField(core, p), p.GetCustomAttribute<BextFieldAttribute>()))
                .Where(t => t.Item2 != null)
                .Where(t=> SuppressFields.Contains(t.Item2.Field)==false) // don't include fields in the suppress list
                )
            {
                if (info.Item2.ValueWithinLengthLimit(info.Item1) == false)
                {
                    throw new BextMetadataException("Value for bext field {0} ('{1}') exceeds maximum length ({2})", info.Item2.Field, info.Item1, info.Item2.MaxLength);
                }

                args.Add($"--{info.Item2.Field}={info.Item1.NormalizeForCommandLine().ToQuoted()}");
            }

            args.Add(Path.Combine(BaseProcessingDirectory, model.GetFolderName(), model.ToFileName()).ToQuoted());

            return string.Join(" ", args);
        }

        private static string GetValueFromField(BextMetadata core, PropertyInfo info)
        {
            return info.GetValue(core).ToDefaultIfEmpty();
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