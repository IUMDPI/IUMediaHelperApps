using System.IO;

namespace Packager.Utilities
{
    public interface IHasher
    {
        string Hash(Stream content);
    }
}