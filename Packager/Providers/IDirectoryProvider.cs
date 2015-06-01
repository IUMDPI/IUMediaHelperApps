using System.Collections.Generic;
using System.IO;

namespace Packager.Providers
{
    public interface IDirectoryProvider
    {
        IEnumerable<string> EnumerateFiles(string path);
    }

    internal class DirectoryProvider : IDirectoryProvider
    {
        public IEnumerable<string> EnumerateFiles(string path)
        {
            return Directory.EnumerateDirectories(path);
        }
    }
}