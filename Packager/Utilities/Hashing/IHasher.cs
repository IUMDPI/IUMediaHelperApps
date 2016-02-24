using System.Threading;
using System.Threading.Tasks;
using Packager.Models.FileModels;

namespace Packager.Utilities.Hashing
{
    public interface IHasher
    {
        Task<string> Hash(AbstractFile model, CancellationToken cancellationToken);
        Task<string> Hash(string path, CancellationToken cancellationToken);
    }
}