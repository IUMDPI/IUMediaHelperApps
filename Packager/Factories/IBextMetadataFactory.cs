using System.Collections.Generic;
using Packager.Models.EmbeddedMetadataModels;
using Packager.Models.FileModels;
using Packager.Models.PodMetadataModels;

namespace Packager.Factories
{
    public interface IBextMetadataFactory
    {
        EmbeddedAudioMetadata Generate(List<ObjectFileModel> models, ObjectFileModel target, AudioPodMetadata metadata);
        
    }
}