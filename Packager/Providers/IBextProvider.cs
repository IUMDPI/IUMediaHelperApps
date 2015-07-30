using System;
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
        Task EmbedBextMetadata(List<ObjectFileModel> instances, ConsolidatedPodMetadata podMetadata, string processingDirectory);
    }

    public class BextProcessor : IBextProcessor
    {
        public BextProcessor(IProgramSettings settings, IProcessRunner processRunner, IXmlExporter xmlExporter, IObserverCollection observers,
            ILookupsProvider lookupsProvider)
        {
            
            BWFMetaEditPath = settings.BWFMetaEditPath;
            ProcessRunner = processRunner;
            XmlExporter = xmlExporter;
            Observers = observers;
            LookupsProvider = lookupsProvider;
        }

        private string BWFMetaEditPath { get; set; }
        private IProcessRunner ProcessRunner { get; set; }
        private IXmlExporter XmlExporter { get; set; }
        private IObserverCollection Observers { get; set; }
        private ILookupsProvider LookupsProvider { get; set; }

        public async Task EmbedBextMetadata(List<ObjectFileModel> instances, ConsolidatedPodMetadata podMetadata, string processingDirectory)
        {
            var masterFileModel = instances.GetPreservationOrIntermediateModel();
            var defaultProvenance = podMetadata.DigitalProvenance.GetFileProvenance(masterFileModel);
            if (defaultProvenance == null)
            {
                throw new AddMetadataException("No digital file provenance in metadata for {0}", masterFileModel.ToFileName());
            }

            var xml = new ConformancePointDocument
            {
                File = (from model in instances
                    let digitizedOn = GetOriginationDateTime(podMetadata, model, defaultProvenance)
                    let description = GetBextDescription(podMetadata, model)
                    select new ConformancePointDocumentFile
                    {
                        Name = Path.Combine(processingDirectory, model.ToFileName()),
                        Core = new ConformancePointDocumentFileCore
                        {
                            Originator = podMetadata.DigitizingEntity,
                            OriginatorReference = Path.GetFileNameWithoutExtension(model.ToFileName()),
                            Description = description,
                            ICMT = description,
                            IARL = GenerateIarl(podMetadata.Unit),
                            OriginationDate = digitizedOn.ToString("yyyy-MM-dd"),
                            OriginationTime = digitizedOn.ToString("HH:mm:ss"),
                            TimeReference = "0",
                            ICRD = digitizedOn.ToString("yyyy-MM-dd"),
                            INAM = podMetadata.Title
                        }
                    }).ToArray()
            };

            await AddMetadata(xml, processingDirectory);
        }

        private static DateTime GetOriginationDateTime(ConsolidatedPodMetadata podMetadata, AbstractFileModel model, DigitalFileProvenance defaultProvenance)
        {
            var provenance = podMetadata.DigitalProvenance.GetFileProvenance(model, defaultProvenance);
            
            DateTime result;
            if (DateTime.TryParse(provenance.DateDigitized, out result) == false)
            {
                throw new AddMetadataException("Could not convert {0} to date time object", provenance.DateDigitized);
            }

            return result;
        }

        private string GenerateIarl(string value)
        {
            return string.Format("{0}.", LookupsProvider.LookupValue(LookupTables.Units, value));
        }

        private static string GetBextDescription(ConsolidatedPodMetadata metadata, ObjectFileModel fileModel)
        {
            return string.Format("{0}. {1}. File use: {2}. {3}",
                metadata.Unit,
                metadata.CallNumber,
                fileModel.FullFileUse, 
                Path.GetFileNameWithoutExtension(fileModel.ToFileName()));
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