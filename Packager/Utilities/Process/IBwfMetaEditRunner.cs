using System.Collections.Generic;
using System.Threading.Tasks;
using Packager.Models.FileModels;
using Packager.Models.ResultModels;
using Packager.Utilities.Bext;

namespace Packager.Utilities.Process
{
    public interface IBwfMetaEditRunner
    {
        string BwfMetaEditPath { get; }
        //Task<IProcessResult> AddMetadata(ObjectFileModel model, BextMetadata core);
        //Task<IProcessResult> ClearMetadata(ObjectFileModel model);

        Task<IProcessResult> ClearMetadata(AbstractFile model, IEnumerable<BextFields> fields);
        Task<string> GetVersion();
    }
}