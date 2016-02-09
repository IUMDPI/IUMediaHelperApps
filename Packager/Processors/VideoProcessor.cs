using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Packager.Extensions;
using Packager.Factories;
using Packager.Models.FileModels;
using Packager.Models.PodMetadataModels;
using Packager.Providers;
using Packager.Utilities.Process;

namespace Packager.Processors
{
    public class VideoProcessor : AbstractProcessor
    {
        public VideoProcessor(IDependencyProvider dependencyProvider) : base(dependencyProvider)
        {
            MetadataFactory = dependencyProvider.VideoMetadataFactory;
        }

        private IFFMPEGRunner FFPMPEGRunner => DependencyProvider.VideoFFMPEGRunner;
        private IFFProbeRunner FFProbeRunner => DependencyProvider.FFProbeRunner;

        private IEmbeddedMetadataFactory<VideoPodMetadata> MetadataFactory { get; }

        protected override string OriginalsDirectory => Path.Combine(ProcessingDirectory, "Originals");
        protected override ICarrierDataFactory CarrierDataFactory => DependencyProvider.VideoCarrierDataFactory;

        protected override IFFMPEGRunner FFMpegRunner => DependencyProvider.VideoFFMPEGRunner;

        protected override async Task<AbstractPodMetadata> GetMetadata(List<AbstractFile> filesToProcess)
        {
            return await GetMetadata<VideoPodMetadata>(filesToProcess);
        }

        protected override async Task NormalizeOriginals(List<AbstractFile> originals, AbstractPodMetadata podMetadata)
        {
            foreach (var original in originals)
            {
                var metadata = MetadataFactory.Generate(originals, original, (VideoPodMetadata) podMetadata);
                await FFPMPEGRunner.Normalize(original, metadata);
            }
        }

        protected override async Task<List<AbstractFile>> CreateProdOrMezzDerivatives(List<AbstractFile> models,
            AbstractPodMetadata metadata)
        {
            var results = new List<AbstractFile>();
            foreach (var master in models
                .GroupBy(m => m.SequenceIndicator)
                .Select(g => g.GetPreservationOrIntermediateModel()))
            {
                var derivative = new MezzanineFile(master);
                var embeddedMetadata = MetadataFactory.Generate(models, derivative, (VideoPodMetadata) metadata);
                results.Add(await FFPMPEGRunner.CreateProdOrMezzDerivative(master, derivative, embeddedMetadata));
            }

            return results;
        }

        protected override async Task ClearMetadataFields(List<AbstractFile> processedList)
        {
            // do nothing
            await Task.Run(() => { });
        }

        protected override async Task<List<AbstractFile>> CreateQualityControlFiles(List<AbstractFile> processedList)
        {
            var results = new List<AbstractFile>();
            foreach (
                var model in
                    processedList.Where(m => m.IsPreservationVersion() || m.IsPreservationIntermediateVersion()))
            {
                results.Add(await FFProbeRunner.GenerateQualityControlFile(model));
            }

            return results;
        }

/*
        protected override async Task<List<AbstractFile>> ProcessFileInternal(List<AbstractFile> filesToProcess)
        {
            // fetch, log, and validate metadata
            var metadata = await GetMetadata<VideoPodMetadata>(filesToProcess);

            await NormalizeOriginals(filesToProcess, metadata);

            // verify normalized versions of originals
            await FFPMPEGRunner.Verify(filesToProcess);

            // create list of files to process and add the original files that
            // we know about
            var processedList = new List<AbstractFile>().Concat(filesToProcess)
                .GroupBy(m => m.Filename).Select(g => g.First()).ToList();

            processedList = processedList.Concat(await CreateMezzanineDerivatives(processedList, metadata)).ToList();

            // now remove duplicate entries -- this could happen if mezzanine master
            // already exists
            processedList = processedList.RemoveDuplicates();

            // now create access models
            processedList = processedList.Concat(await CreateAccessDerivatives(processedList)).ToList();

            // create QC files
            var qcFiles = await CreateQualityControlFiles(processedList);
            // add qc files to processed list
            processedList = processedList.Concat(qcFiles).ToList();

            // using the list of files that have been processed
            // make the xml file
            var xmlModel = await GenerateXml<VideoCarrier>(metadata, processedList);

            var outputList = new List<AbstractFile>().Concat(processedList).ToList();
            outputList.Add(xmlModel);

            return outputList;
        }*/

        
        /*  private async Task<List<AbstractFile>> CreateAccessDerivatives(IEnumerable<AbstractFile> models)
        {
            var results = new List<AbstractFile>();

            // for each production master, create an access version
            foreach (var model in models.Where(m => m.IsMezzanineVersion()))
            {
                results.Add(await FFPMPEGRunner.CreateAccessDerivative(model));
            }
            return results;
        }

        private async Task<List<AbstractFile>> CreateQualityControlFiles(IEnumerable<AbstractFile> processedList)
        {
            var results = new List<AbstractFile>();
            foreach (
                var model in
                    processedList.Where(m => m.IsPreservationVersion() || m.IsPreservationIntermediateVersion()))
            {
                results.Add(await FFProbeRunner.GenerateQualityControlFile(model));
            }

            return results;
        }*/
    }
}