using System.IO;
using Packager.Models.FileModels;

namespace Packager.Utilities
{
    public interface IHasher
    {
        string Hash(Stream content);
        string Hash(string path);
        string Hash(AbstractFileModel model);
    }
}