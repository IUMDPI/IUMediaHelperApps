using System.Collections.Generic;
using Packager.Models.BextModels;
using Packager.Models.FileModels;
using Packager.Models.PodMetadataModels;

namespace Packager.Factories
{
    public interface IBextMetadataFactory
    {
        BextMetadata Generate(List<ObjectFileModel> models, ObjectFileModel target, ConsolidatedPodMetadata metadata);
        
    }
}