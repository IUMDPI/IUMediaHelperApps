using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Packager.Models.FileModels;

namespace Packager.Utilities.Bext
{
    public interface IBextProcessor
    {
        Task ClearMetadataFields(List<AbstractFile> instances, List<BextFields> fields, CancellationToken cancellationToken);
    }
}