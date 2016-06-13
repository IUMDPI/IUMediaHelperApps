using System.Threading;
using System.Threading.Tasks;
using Packager.Models.FileModels;

namespace Packager.Providers
{
    public interface IMediaInfoProvider
    {
        Task SetMediaInfo(AbstractFile target, CancellationToken cancellationToken);
    }
}
