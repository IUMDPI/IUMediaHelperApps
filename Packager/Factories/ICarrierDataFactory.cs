using System.Collections.Generic;
using Packager.Models.FileModels;
using Packager.Models.OutputModels.Carrier;
using Packager.Models.PodMetadataModels;

namespace Packager.Factories
{
    public interface ICarrierDataFactory
    {
        /*AudioCarrier Generate(AudioPodMetadata excelModel, List<AbstractFile> filesToProcess);
        VideoCarrier Generate(VideoPodMetadata excelModel, List<AbstractFile> filesToProcess);
*/
        T Generate<T>(AbstractPodMetadata metadata, List<AbstractFile> filesToProcess) where T : AbstractCarrierData;
    }
}