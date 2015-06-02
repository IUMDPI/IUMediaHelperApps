using System.Collections.Generic;
using System.IO;

namespace Packager.Providers
{
    public interface IDirectoryProvider
    {
        IEnumerable<string> EnumerateFiles(string path);
        DirectoryInfo CreateDirectory(string path);
    }

    internal class DirectoryProvider : IDirectoryProvider
    {
        public IEnumerable<string> EnumerateFiles(string path)
        {
            return Directory.EnumerateFiles(path);
        }

        public DirectoryInfo CreateDirectory(string path)
        {
            return Directory.CreateDirectory(path);
        }
    }
}