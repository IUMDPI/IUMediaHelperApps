using System.Collections.Generic;
using Packager.Models.EmbeddedMetadataModels;
using Packager.Models.FileModels;
using Packager.Models.PodMetadataModels;

namespace Packager.Factories
{
    public interface IEmbeddedMetadataFactory<in T> where T : AbstractPodMetadata
    {
        AbstractEmbeddedMetadata Generate(IEnumerable<ObjectFileModel> models, ObjectFileModel target, T metadata);
    }
}