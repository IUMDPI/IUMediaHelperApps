using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Packager.Exceptions;
using Packager.Extensions;
using Packager.Factories;
using Packager.Models.BextModels;
using Packager.Models.FileModels;
using Packager.Models.PodMetadataModels;
using Packager.Models.ResultModels;
using Packager.Observers;
using Packager.Validators.Attributes;
using Packager.Verifiers;

namespace Packager.Utilities
{
    public class BextProcessor : IBextProcessor
    {


        public BextProcessor(string bwfMetaEditPath, string baseProcessingDirectory, IProcessRunner processRunner, IXmlExporter xmlExporter, IObserverCollection observers, IBwfMetaEditResultsVerifier verifier,
            IConformancePointDocumentFactory conformancePointDocumentFactory)
        {
            BwfMetaEditPath = bwfMetaEditPath;
            BaseProcessingDirectory = baseProcessingDirectory;
            ProcessRunner = processRunner;
            XmlExporter = xmlExporter;
            Observers = observers;
            Verifier = verifier;
            ConformancePointDocumentFactory = conformancePointDocumentFactory;
        }

        private IProcessRunner ProcessRunner { get; set; }
        private IXmlExporter XmlExporter { get; set; }
        private IObserverCollection Observers { get; set; }
        private IBwfMetaEditResultsVerifier Verifier { get; set; }
        private IConformancePointDocumentFactory ConformancePointDocumentFactory { get; set; }

        [ValidateFile]
        public string BwfMetaEditPath { get; set; }

        [ValidateFolder]
        private string BaseProcessingDirectory { get; set; }

        public async Task EmbedBextMetadata(List<ObjectFileModel> instances, ConsolidatedPodMetadata podMetadata)
        {
            var files = new List<ConformancePointDocumentFile>();
            foreach (var fileModel in instances)
            {
                var defaultProvenance = GetDefaultProvenance(instances, podMetadata, fileModel);
                var provenance = podMetadata.FileProvenances.GetFileProvenance(fileModel, defaultProvenance);
                files.Add(ConformancePointDocumentFactory.Generate(fileModel, provenance, podMetadata));
            }

            var xml = new ConformancePointDocument
            {
                File = files.ToArray()
            };

            await AddMetadata(xml, instances.First().GetFolderName());
        }

        public async Task ClearBextMetadataField(List<ObjectFileModel> instances, BextFields field)
        {
            for (var i = 0; i < instances.Count; i++)
            {
                var instance = instances[i];
                var path = Path.Combine(BaseProcessingDirectory, instance.GetFolderName(), instance.ToFileName());
                var args = $"--verbose --Append --{field}=\"\" {path.ToQuoted()}";

                var result = await ExecuteBextProcess(args);

                if (i > 0)
                {
                    Observers.Log("");
                }

                Observers.Log(FormatOutput(result.StandardOutput, instance.GetFolderName()));

                if (Verifier.Verify(result.StandardOutput.ToLowerInvariant(), instance.ToFileName().ToLowerInvariant(), Observers) == false)
                {
                    throw new BextMetadataException("Could not clear metadata field {0} in one or more files!", field);
                }
            }

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

        public async Task<string> GetBwfMetaEditVersion()
        {
            try
            {
                var startInfo = new ProcessStartInfo(BwfMetaEditPath) { Arguments = "--version" };
                var result = await ProcessRunner.Run(startInfo);

                var parts = result.StandardOutput.Split(new[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
                return parts.Last();
            }
            catch (Exception)
            {
                return "";
            }
        }

        private static DigitalFileProvenance GetDefaultProvenance(IEnumerable<ObjectFileModel> instances, ConsolidatedPodMetadata podMetadata, ObjectFileModel model)
        {
            var sequenceInstances = instances.Where(m => m.SequenceIndicator.Equals(model.SequenceIndicator));
            var sequenceMaster = sequenceInstances.GetPreservationOrIntermediateModel();
            if (sequenceMaster == null)
            {
                throw new BextMetadataException("No corresponding preservation or preservation-intermediate master present for {0}", model.ToFileName());
            }

            var defaultProvenance = podMetadata.FileProvenances.GetFileProvenance(sequenceMaster);
            if (defaultProvenance == null)
            {
                throw new BextMetadataException("No digital file provenance in metadata for {0}", sequenceMaster.ToFileName());
            }

            return defaultProvenance;
        }

        private async Task AddMetadata(ConformancePointDocument xml, string objectFolder)
        {
            var xmlPath = Path.Combine(BaseProcessingDirectory, objectFolder, "core.xml");
            XmlExporter.ExportToFile(xml, xmlPath, Encoding.ASCII);

            var args = $"--verbose --Append --in-core={xmlPath.ToQuoted()}";

            var result = await ExecuteBextProcess(args);

            Observers.Log(FormatOutput(result.StandardOutput, objectFolder));

            if (!Verifier.Verify(result.StandardOutput.ToLowerInvariant(),
                xml.File.Select(f => f.Name.ToLowerInvariant()).ToList(), Observers))
            {
                throw new BextMetadataException("Could not add metadata to one or more files!");
            }
        }

        private static string FormatOutput(string output, string objectFolder)
        {
            var builder = new StringBuilder();
            var lines = output.Split(new[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
            for (var index = 0; index < lines.Length; index++)
            {
                if (index > 0 && lines[index].EndsWith(": Is open"))
                {
                    builder.Append('\n');
                }

                var text = NormalizeLogLine(lines[index], objectFolder);
                builder.AppendFormat("{0}\n", text);
            }

            return builder.ToString();
        }


        private static string NormalizeLogLine(string line, string objectFolder)
        {
            if (string.IsNullOrWhiteSpace(line))
            {
                return line;
            }

            // want to remove everything before the file name
            // so find the base processing directory in the string
            // and start from there + its length +1
            var index = line.IndexOf(objectFolder, StringComparison.InvariantCultureIgnoreCase) + objectFolder.Length + 1;
            if (index <= 0 || index >= line.Length)
            {
                return line;
            }

            return line.Substring(index);
        }
    }
}