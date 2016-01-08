﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Packager.Exceptions;
using Packager.Extensions;
using Packager.Factories;
using Packager.Models.FileModels;
using Packager.Models.OutputModels;
using Packager.Models.PodMetadataModels;
using Packager.Providers;
using Packager.Utilities;
using Packager.Utilities.Process;

namespace Packager.Processors
{
    internal class VideoProcessor : AbstractProcessor
    {
        public VideoProcessor(IDependencyProvider dependencyProvider) : base(dependencyProvider)
        {
            MetadataFactory = dependencyProvider.VideoMetadataFactory;
        }

        private IFFMPEGRunner FFPMPEGRunner => DependencyProvider.VideoFFMPEGRunner;
        private IFFProbeRunner FFProbeRunner => DependencyProvider.FFProbeRunner;

        private IEmbeddedMetadataFactory<VideoPodMetadata> MetadataFactory { get; }

        protected override string OriginalsDirectory => Path.Combine(ProcessingDirectory, "Originals");

        protected override async Task<IEnumerable<AbstractFileModel>> ProcessFileInternal(List<ObjectFileModel> filesToProcess)
        {
            // fetch, log, and validate metadata
            var metadata = await GetMetadata<VideoPodMetadata>(filesToProcess);

            await NormalizeOriginals(filesToProcess, metadata);

            // verify normalized versions of originals
            await FFPMPEGRunner.Verify(filesToProcess);

            // create list of files to process and add the original files that
            // we know about
            var processedList = new List<ObjectFileModel>().Concat(filesToProcess)
                .GroupBy(m => m.ToFileName()).Select(g => g.First()).ToList();

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
            var xmlModel = await GenerateXml(metadata, processedList);

            var outputList = new List<AbstractFileModel>().Concat(processedList).ToList();
            outputList.Add(xmlModel);
            
            return outputList;
        }

        private async Task<XmlFileModel> GenerateXml(VideoPodMetadata metadata, List<ObjectFileModel> filesToProcess)
        {
            var result = new XmlFileModel { BarCode = Barcode, ProjectCode = ProjectCode };
            var sectionKey = Observers.BeginSection("Generating {0}", result.ToFileName());
            try
            {
                await AssignChecksumValues(filesToProcess);

                var wrapper = new IU { Carrier = MetadataGenerator.Generate(metadata, filesToProcess) };
                XmlExporter.ExportToFile(wrapper, Path.Combine(ProcessingDirectory, result.ToFileName()));

                Observers.EndSection(sectionKey, $"{result.ToFileName()} generated successfully");
                return result;
            }
            catch (Exception e)
            {
                Observers.EndSection(sectionKey);
                Observers.LogProcessingIssue(e, Barcode);
                throw new LoggedException(e);
            }
        }

        protected async Task AssignChecksumValues(IEnumerable<ObjectFileModel> models)
        {
            foreach (var model in models)
            {
                model.Checksum = await Hasher.Hash(model);
                Observers.Log("{0} checksum: {1}", Path.GetFileNameWithoutExtension(model.ToFileName()), model.Checksum);
            }
        }

        private async Task NormalizeOriginals(List<ObjectFileModel> originals, VideoPodMetadata podMetadata)
        {
            foreach (var original in originals)
            {
                var metadata = MetadataFactory.Generate(originals, original, podMetadata);
                await FFPMPEGRunner.Normalize(original, metadata);
            }
        }

        private async Task<List<ObjectFileModel>> CreateMezzanineDerivatives(List<ObjectFileModel> models, VideoPodMetadata metadata)
        {
            var results = new List<ObjectFileModel>();
            foreach (var master in models
                .GroupBy(m => m.SequenceIndicator)
                .Select(g => g.GetPreservationOrIntermediateModel()))
            {
                var derivative = master.ToMezzanineFileModel();
                var embeddedMetadata = MetadataFactory.Generate(models, derivative, metadata);
                results.Add(await FFPMPEGRunner.CreateProdOrMezzDerivative(master, derivative, embeddedMetadata));
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

        private async Task<List<ObjectFileModel>> CreateQualityControlFiles(IEnumerable<ObjectFileModel> processedList)
        {
            var results = new List<ObjectFileModel>();
            foreach (var model in processedList.Where(m=> m.IsPreservationVersion() || m.IsPreservationIntermediateVersion()))
            {
                results.Add(await FFProbeRunner.GenerateQualityControlFile(model));
            }

            return results;
        }
    }
}