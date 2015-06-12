using System.Collections.Generic;
using Packager.Models;
using Packager.Models.FileModels;
using Packager.Models.OutputModels;
using Packager.Models.PodMetadataModels;

namespace Packager.Utilities
{
    public interface IMetadataGenerator
    {
        CarrierData GenerateMetadata(ConsolidatedPodMetadata excelModel, IEnumerable<ObjectFileModel> filesToProcess, string processingDirectory);
    }
}