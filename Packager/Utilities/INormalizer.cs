using System.Collections.Generic;
using Packager.Models.FileModels;

namespace Packager.Utilities
{
    public interface INormalizer
    {
        void Normalize(List<ObjectFileModel> originals, string originalsFolder, string processingFolder);

        void Verify(List<ObjectFileModel> originals, string originalsFolder, string processingFolder);
    }
}