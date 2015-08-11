using System.Collections.Generic;
using Packager.Models.FileModels;
using Packager.Models.OutputModels;
using Packager.Models.PodMetadataModels;

namespace Packager.Factories
{
    public interface ICarrierDataFactory
    {
        CarrierData Generate(ConsolidatedPodMetadata excelModel, IEnumerable<ObjectFileModel> filesToProcess, string processingDirectory);
    }
}