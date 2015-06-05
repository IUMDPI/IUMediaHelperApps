using System.Collections.Generic;
using Packager.Models;
using Packager.Models.FileModels;
using Packager.Models.PodMetadataModels;

namespace Packager.Utilities
{
    public interface IMetadataGenerator
    {
        CarrierData GenerateMetadata(PodMetadata excelModel, IEnumerable<ObjectFileModel> filesToProcess, string processingDirectory);
    }
}