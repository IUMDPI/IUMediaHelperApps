using System.Collections.Generic;
using Packager.Models.FileModels;
using Packager.Models.OutputModels;
using Packager.Models.PodMetadataModels;

namespace Packager.Factories
{
    public interface ISideDataFactory
    {
        SideData[] Generate(AudioPodMetadata podMetadata, IEnumerable<ObjectFileModel> filesToProcess);
        SideData[] Generate(VideoPodMetadata podMetadata, IEnumerable<ObjectFileModel> filesToProcess);
    }
}