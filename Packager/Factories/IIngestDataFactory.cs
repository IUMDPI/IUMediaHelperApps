﻿using Packager.Models.FileModels;
using Packager.Models.OutputModels;
using Packager.Models.PodMetadataModels;
using Packager.Models.PodMetadataModels.ConsolidatedModels;

namespace Packager.Factories
{
    public interface IIngestDataFactory
    {
        IngestData Generate(ConsolidatedPodMetadata podMetadata, AbstractFileModel masterFileModel);
    }
}