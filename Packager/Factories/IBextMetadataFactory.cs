using System.Collections.Generic;
using Packager.Models.BextModels;
using Packager.Models.FileModels;
using Packager.Models.PodMetadataModels;
using Packager.Models.PodMetadataModels.ConsolidatedModels;

namespace Packager.Factories
{
    public interface IBextMetadataFactory
    {
        BextMetadata Generate(List<ObjectFileModel> models, ObjectFileModel target, ConsolidatedAudioPodMetadata metadata);
        
    }
}