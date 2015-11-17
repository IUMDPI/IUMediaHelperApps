using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Packager.Extensions;
using Packager.Models.FileModels;
using Packager.Models.PodMetadataModels;
using Packager.Providers;
using Packager.Utilities;

namespace Packager.Processors
{
    internal class VideoProcessor : AbstractProcessor
    {
        public VideoProcessor(IDependencyProvider dependencyProvider) : base(dependencyProvider)
        {
        }

        private IVideoFFMPEGRunner FFPMPEGRunner => DependencyProvider.VideoFFMPEGRunner;

        protected override string OriginalsDirectory => ProcessingDirectory;

        protected override async Task<IEnumerable<AbstractFileModel>> ProcessFileInternal(List<ObjectFileModel> filesToProcess)
        {
            // fetch, log, and validate metadata
            var metadata = await GetMetadata<VideoPodMetadata>(filesToProcess);

            // create list of files to process and add the original files that
            // we know about
            var processedList = new List<ObjectFileModel>().Concat(filesToProcess)
                .GroupBy(m => m.ToFileName()).Select(g => g.First()).ToList();

            processedList = processedList.Concat(await CreateMezzanineDerivatives(processedList, metadata)).ToList();

            // now remove duplicate entries -- this could happen if mezzanine master
            // already exists
            processedList = processedList.RemoveDuplicates();

            // now embed metadata
            await FFPMPEGRunner.EmbedMetadata(processedList, metadata);

            // now create access models
            processedList = processedList.Concat(await CreateAccessDerivatives(processedList)).ToList();

            // create QC files
            var qcFiles = await CreateQualityControlFiles(processedList);

            // concat processed list and output list and return
            return new List<AbstractFileModel>().Concat(processedList).Concat(qcFiles).ToList();
        }
        
        private async Task<List<ObjectFileModel>> CreateMezzanineDerivatives(List<ObjectFileModel> models, VideoPodMetadata metadata)
        {
            var results = new List<ObjectFileModel>();
            foreach (var master in models
                .GroupBy(m => m.SequenceIndicator)
                .Select(g => g.GetPreservationOrIntermediateModel()))
            {
                var derivative = master.ToMezzanineFileModel();
                results.Add(await FFPMPEGRunner.CreateMezzanineDerivative(master, derivative));
            }

            return results;
        }

        private async Task<List<ObjectFileModel>> CreateAccessDerivatives(IEnumerable<ObjectFileModel> models)
        {
            var results = new List<ObjectFileModel>();

            // for each production master, create an access version
            foreach (var model in models.Where(m => m.IsMezzanineVersion()))
            {
                results.Add(await FFPMPEGRunner.CreateAccessDerivative(model));
            }
            return results;
        }

        private async Task<List<AbstractFileModel>> CreateQualityControlFiles(List<ObjectFileModel> processedList)
        {
            //todo: implement
            return new List<AbstractFileModel>();
        }
    }
}