using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Packager.Exceptions;
using Packager.Extensions;
using Packager.Models;
using Packager.Models.BextModels;
using Packager.Models.FileModels;
using Packager.Models.PodMetadataModels;
using Packager.Observers;
using Packager.Utilities;
using Packager.Verifiers;

namespace Packager.Providers
{
    public interface IBextProcessor
    {
        Task EmbedBextMetadata(IEnumerable<ObjectFileModel> instances, ConsolidatedPodMetadata podMetadata, string processingDirectory);
    }

    public class BextProcessor : IBextProcessor
    {
        private IFileProvider FileProvider { get; set; }
        private string DigitizingEntity { get; set; }
        private string BWFMetaEditPath { get; set; }
        private IProcessRunner ProcessRunner { get; set; }
        private IXmlExporter XmlExporter { get; set; }
        private IObserverCollection Observers { get; set; }

        public BextProcessor(IProgramSettings settings, IFileProvider fileProvider, IProcessRunner processRunner, IXmlExporter xmlExporter, IObserverCollection observers)
        {
            FileProvider = fileProvider;
            DigitizingEntity = settings.DigitizingEntity;
            BWFMetaEditPath = settings.BWFMetaEditPath;
            ProcessRunner = processRunner;
            XmlExporter = xmlExporter;
            Observers = observers;
        }
        
        public async Task EmbedBextMetadata(IEnumerable<ObjectFileModel> instances, ConsolidatedPodMetadata podMetadata, string processingDirectory)
        {
            var xml = new ConformancePointDocumentFactory(FileProvider, processingDirectory, DigitizingEntity)
                   .Get(instances, podMetadata);

            await AddMetadata(xml, processingDirectory);
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

            Observers.Log(result.StandardOutput);

            var verifier = new BwfMetaEditResultsVerifier(
                result.StandardOutput.ToLowerInvariant(),
                xml.File.Select(f => f.Name.ToLowerInvariant()).ToList(),
                Observers);

            if (!verifier.Verify())
            {
                throw new AddMetadataException("Could not add metadata to one or more files!");
            }
        }
    }
}