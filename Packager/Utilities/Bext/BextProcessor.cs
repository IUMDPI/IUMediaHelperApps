using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Packager.Exceptions;
using Packager.Extensions;
using Packager.Models.FileModels;
using Packager.Observers;
using Packager.Utilities.Process;
using Packager.Verifiers;

namespace Packager.Utilities.Bext
{
    public class BextProcessor : IBextProcessor
    {
        public BextProcessor(IBwfMetaEditRunner metaEditRunner, IObserverCollection observers, IBwfMetaEditResultsVerifier verifier)
        {
            MetaEditRunner = metaEditRunner;
            Observers = observers;
            Verifier = verifier;
        }

        public IBwfMetaEditRunner MetaEditRunner { get; set; }
        private IObserverCollection Observers { get; }
        private IBwfMetaEditResultsVerifier Verifier { get; }

        public async Task ClearMetadataFields(List<AbstractFile> instances, List<BextFields> fields)
        {
            foreach (var instance in instances)
            {
                var result = await MetaEditRunner.ClearMetadata(instance, fields);

                if (instances.IsFirst(instance) == false)
                {
                    Observers.Log("");
                }

                if (Verifier.Verify(result.StandardOutput.GetContent().ToLowerInvariant(), instance.ToFileName().ToLowerInvariant()) == false)
                {
                    Observers.Log(result.StandardOutput.GetContent());
                    throw new EmbeddedMetadataException("Could not clear metadata fields for {0}", instance.ToFileName());
                }

                Observers.Log(FormatOutput(result.StandardOutput.GetContent(), instance.GetFolderName()));
            }
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