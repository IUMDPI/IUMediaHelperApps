using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Packager.Models.FileModels;
using Packager.Models.ResultModels;
using Packager.Utilities.Bext;

namespace Packager.Utilities.ProcessRunners
{
    public interface IBwfMetaEditRunner
    {
        string BwfMetaEditPath { get; }
        Task<IProcessResult> ClearMetadata(AbstractFile model, IEnumerable<BextFields> fields, CancellationToken cancellationToken);
        Task<string> GetVersion();
    }
}