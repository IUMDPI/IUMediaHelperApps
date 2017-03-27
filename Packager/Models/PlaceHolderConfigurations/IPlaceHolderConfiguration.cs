using System.Collections.Generic;
using Packager.Models.FileModels;

namespace Packager.Models.PlaceHolderConfigurations
{
    public interface IPlaceHolderConfiguration
    {
        List<AbstractFile> GetPlaceHoldersToAdd(List<AbstractFile> fileModels);
    }
}
