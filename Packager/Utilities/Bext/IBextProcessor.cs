using System.Collections.Generic;
using System.Threading.Tasks;
using Packager.Models.FileModels;

namespace Packager.Utilities.Bext
{
    public interface IBextProcessor
    {
        Task ClearMetadataFields(List<ObjectFileModel> instances, List<BextFields> fields);
    }
}