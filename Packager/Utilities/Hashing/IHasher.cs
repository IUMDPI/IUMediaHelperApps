using System.Threading.Tasks;
using Packager.Models.FileModels;

namespace Packager.Utilities.Hashing
{
    public interface IHasher
    {
        Task<string> Hash(AbstractFile model);
        Task<string> Hash(string path);
    }
}