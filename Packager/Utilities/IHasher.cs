using System.IO;
using System.Threading.Tasks;
using Packager.Models.FileModels;

namespace Packager.Utilities
{
    public interface IHasher
    {
        Task<string> Hash(AbstractFileModel model);
    }
}