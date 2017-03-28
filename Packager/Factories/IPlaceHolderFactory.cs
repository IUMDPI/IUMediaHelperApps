using System.Collections.Generic;
using Packager.Models.FileModels;

namespace Packager.Factories
{
    public interface IPlaceHolderFactory
    {
        List<AbstractFile> GetPlaceHoldersToAdd(string format, List<AbstractFile> fileModels);
    }
}
