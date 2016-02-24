using System.Collections.Generic;
using Packager.Models.FileModels;
using Packager.Models.OutputModels.Carrier;
using Packager.Models.PodMetadataModels;

namespace Packager.Factories
{
    public interface ICarrierDataFactory<in T> where T:AbstractPodMetadata
    {
        AbstractCarrierData Generate(T metadata, string digitizingEntity, List<AbstractFile> filesToProcess);
    }
}