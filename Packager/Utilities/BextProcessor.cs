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
using Packager.Models;
using Packager.Models.BextModels;
using Packager.Models.FileModels;
using Packager.Models.PodMetadataModels;
using Packager.Observers;
using Packager.Verifiers;

namespace Packager.Utilities
{
    public class BextProcessor : IBextProcessor
    {
        public BextProcessor(IProgramSettings settings, IProcessRunner processRunner, IXmlExporter xmlExporter, IObserverCollection observers, IBwfMetaEditResultsVerifier verifier, IConformancePointDocumentFactory conformancePointDocumentFactory)
        {

            BWFMetaEditPath = settings.BWFMetaEditPath;
            ProcessRunner = processRunner;
            XmlExporter = xmlExporter;
            Observers = observers;
            Verifier = verifier;
            ConformancePointDocumentFactory = conformancePointDocumentFactory;

        }

        private string BWFMetaEditPath { get; set; }
        private IProcessRunner ProcessRunner { get; set; }
        private IXmlExporter XmlExporter { get; set; }
        private IObserverCollection Observers { get; set; }
        private IBwfMetaEditResultsVerifier Verifier { get; set; }
        private IConformancePointDocumentFactory ConformancePointDocumentFactory { get; set; }


        public async Task EmbedBextMetadata(List<ObjectFileModel> instances, ConsolidatedPodMetadata podMetadata, string processingDirectory)
        {
            var files = new List<ConformancePointDocumentFile>();
            foreach (var fileModel in instances)
            {
                var defaultProvenance = GetDefaultProvenance(instances, podMetadata, fileModel);
                var provenance = podMetadata.FileProvenances.GetFileProvenance(fileModel, defaultProvenance);
                files.Add(ConformancePointDocumentFactory.Generate(fileModel, provenance, podMetadata, processingDirectory));
            }

            var xml = new ConformancePointDocument
            {
                File = files.ToArray()
            };

            await AddMetadata(xml, processingDirectory);
        }

        private static DigitalFileProvenance GetDefaultProvenance(IEnumerable<ObjectFileModel> instances, ConsolidatedPodMetadata podMetadata, ObjectFileModel model)
        {
            var sequenceInstances = instances.Where(m => m.SequenceIndicator.Equals(model.SequenceIndicator));
            var sequenceMaster = sequenceInstances.GetPreservationOrIntermediateModel();
            if (sequenceMaster == null)
            {
                throw new AddMetadataException("No corresponding preservation or preservation-intermediate master present for {0}", model.ToFileName());
            }

            var defaultProvenance = podMetadata.FileProvenances.GetFileProvenance(sequenceMaster);
            if (defaultProvenance == null)
            {
                throw new AddMetadataException("No digital file provenance in metadata for {0}", sequenceMaster.ToFileName());
            }

            return defaultProvenance;
        }

        private async Task AddMetadata(ConformancePointDocument xml, string processingDirectory)
        {
            var xmlPath = Path.Combine(processingDirectory, "core.xml");
            XmlExporter.ExportToFile(xml, xmlPath);

            var args = string.Format("--verbose --Append --in-core={0}", xmlPath.ToQuoted());

            var startInfo = new ProcessStartInfo(BWFMetaEditPath)
            {
                Arguments = args,
                RedirectStandardError = true,
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            var result = await ProcessRunner.Run(startInfo);

            Observers.Log(FormatOutput(result.StandardOutput));

            if (!Verifier.Verify(result.StandardOutput.ToLowerInvariant(),
                xml.File.Select(f => f.Name.ToLowerInvariant()).ToList(), Observers))
            {
                throw new AddMetadataException("Could not add metadata to one or more files!");
            }
        }

        private static string FormatOutput(string output)
        {
            var builder = new StringBuilder();
            var lines = output.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
            for (var index = 0; index < lines.Count(); index++)
            {
                if (index > 0 && lines[index].EndsWith(": Is open"))
                {
                    builder.Append('\n');
                }

                builder.AppendFormat("{0}\n", lines[index]);
            }

            return builder.ToString();
        }
    }
}