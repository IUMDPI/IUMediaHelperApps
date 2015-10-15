using System.Collections.Generic;
using Packager.Models.FileModels;

namespace Packager.Utilities
{
    public interface INormalizer
    {
        void Normalize(List<ObjectFileModel> originals, string originalsFolder, string processingFolder);

        void Verify(List<ObjectFileModel> originals, string originalsFolder, string processingFolder);
    }

    class FFMpegNormalizer : INormalizer
    {


        public void Normalize(List<ObjectFileModel> originals, string originalsFolder, string processingFolder)
        {
            throw new System.NotImplementedException();
        }

        public void Verify(List<ObjectFileModel> originals, string originalsFolder, string processingFolder)
        {
            throw new System.NotImplementedException();
        }
    }
}