using System.Threading.Tasks;
using Packager.Models.FileModels;

namespace Packager.Utilities.Hashing
{
    public interface IHasher
    {
        Task<string> Hash(AbstractFileModel model);
        Task<string> Hash(string path);
    }
}