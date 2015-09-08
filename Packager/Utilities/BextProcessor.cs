using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Packager.Exceptions;
using Packager.Extensions;
using Packager.Factories;
using Packager.Models.FileModels;
using Packager.Models.PodMetadataModels;
using Packager.Observers;
using Packager.Verifiers;

namespace Packager.Utilities
{
    public class BextProcessor : IBextProcessor
    {
        public BextProcessor(IBwfMetaEditRunner metaEditRunner, IObserverCollection observers, IBwfMetaEditResultsVerifier verifier,
            IBextMetadataFactory conformancePointDocumentFactory)
        {
            MetaEditRunner = metaEditRunner;
            Observers = observers;
            Verifier = verifier;
            ConformancePointDocumentFactory = conformancePointDocumentFactory;
        }

        public IBwfMetaEditRunner MetaEditRunner { get; set; }
        private IObserverCollection Observers { get; }
        private IBwfMetaEditResultsVerifier Verifier { get; }
        private IBextMetadataFactory ConformancePointDocumentFactory { get; }


        public async Task EmbedBextMetadata(List<ObjectFileModel> instances, ConsolidatedPodMetadata podMetadata)
        {
            //var files = new List<ConformancePointDocumentFile>();
            foreach (var fileModel in instances)
            {
                var defaultProvenance = GetDefaultProvenance(instances, podMetadata, fileModel);
                var provenance = podMetadata.FileProvenances.GetFileProvenance(fileModel, defaultProvenance);
                var core = ConformancePointDocumentFactory.Generate(fileModel, provenance, podMetadata);
                var result = await MetaEditRunner.AddMetadata(fileModel, core);

                if (instances.IsFirst(fileModel) == false)
                {
                    Observers.Log("");
                }

                if (Verifier.Verify(result.StandardOutput.ToLowerInvariant(), fileModel.ToFileName().ToLowerInvariant()) == false)
                {
                    Observers.Log(result.StandardOutput);
                    throw new BextMetadataException("Could not add bext metadata to {0}", fileModel.ToFileName());
                }

                Observers.Log(FormatOutput(result.StandardOutput, fileModel.GetFolderName()));
            }
        }

        public async Task ClearAllBextMetadataFields(List<ObjectFileModel> instances)
        {
            foreach (var instance in instances)
            {
                var result = await MetaEditRunner.ClearMetadata(instance);

                if (instances.IsFirst(instance) == false)
                {
                    Observers.Log("");
                }

                if (Verifier.Verify(result.StandardOutput.ToLowerInvariant(), instance.ToFileName().ToLowerInvariant()) == false)
                {
                    Observers.Log(result.StandardOutput);
                    throw new BextMetadataException("Could not clear metadata fields for {0}", instance.ToFileName());
                }

                Observers.Log(FormatOutput(result.StandardOutput, instance.GetFolderName()));
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

        private static string FormatOutput(string output, string objectFolder)
        {
            var builder = new StringBuilder();
            var lines = output.Split(new[] {"\r\n"}, StringSplitOptions.RemoveEmptyEntries)
                .Select(l => NormalizeLogLine(l, objectFolder));
            foreach (var line in lines)
            {
                if (IsStandardLine(line) && builder.HasContent())
                {
                    builder.Append("\n");
                }

                if (IsOpeningLine(line) && builder.HasContent())
                {
                    builder.Append("\n");
                }

                builder.Append(line);
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

            // return a newline followed by the formatted line
            return line.Substring(index);
        }

        private static bool IsStandardLine(string line)
        {
            return line.ToLowerInvariant().Contains(".wav:");
        }

        private static bool IsOpeningLine(string line)
        {
            return line.ToLowerInvariant().EndsWith(": is open");
        }
    }
}