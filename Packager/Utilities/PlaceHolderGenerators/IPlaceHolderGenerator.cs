using System.Collections.Generic;
using Packager.Models.FileModels;

namespace Packager.Utilities.PlaceHolderGenerators
{
    public interface IPlaceHolderGenerator
    {
        List<AbstractFile> GetPlaceHoldersToAdd(List<AbstractFile> fileModels);
    }
}
