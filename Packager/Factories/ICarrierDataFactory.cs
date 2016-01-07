using System.Collections.Generic;
using System.Threading.Tasks;
using Packager.Models.FileModels;
using Packager.Models.OutputModels;
using Packager.Models.PodMetadataModels;

namespace Packager.Factories
{
    public interface ICarrierDataFactory
    {
        AudioCarrierData Generate(AudioPodMetadata excelModel, List<ObjectFileModel> filesToProcess);
    }
}