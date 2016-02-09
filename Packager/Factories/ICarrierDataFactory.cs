using System.Collections.Generic;
using Packager.Models.FileModels;
using Packager.Models.OutputModels.Carrier;
using Packager.Models.PodMetadataModels;

namespace Packager.Factories
{
    public interface ICarrierDataFactory
    {
        AbstractCarrierData Generate(AbstractPodMetadata metadata, List<AbstractFile> filesToProcess);
    }
}