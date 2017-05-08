using System.Collections.Generic;
using Common.Models;
using Packager.Models.FileModels;

namespace Packager.Factories
{
    public interface IPlaceHolderFactory
    {
        List<AbstractFile> GetPlaceHoldersToAdd(IMediaFormat format, List<AbstractFile> fileModels);
    }
}
