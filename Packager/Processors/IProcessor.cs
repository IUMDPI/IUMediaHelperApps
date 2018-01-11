using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Packager.Models.FileModels;
using Packager.Models.ResultModels;
using Packager.Validators;

namespace Packager.Processors
{
    public interface IProcessor
    {
        Task<DurationResult> ProcessObject(IGrouping<string, AbstractFile> fileModels, CancellationToken cancellationToken);
    }
}