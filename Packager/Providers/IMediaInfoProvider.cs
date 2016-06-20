using System.Threading;
using System.Threading.Tasks;
using Packager.Models.FileModels;

namespace Packager.Providers
{
    public interface IMediaInfoProvider
    {
        Task<MediaInfo> GetMediaInfo(AbstractFile target, CancellationToken cancellationToken);
    }
}
