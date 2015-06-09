using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Packager.Providers
{
    public interface IDirectoryProvider
    {
        IEnumerable<string> EnumerateFiles(string path);
        DirectoryInfo CreateDirectory(string path);
        Task MoveDirectoryAsync(string sourcePath, string destPath);
        void MoveDirectory(string sourcePath, string destPath);
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

        public async Task MoveDirectoryAsync(string sourcePath, string destPath)
        {
            await Task.Run(() => { Directory.Move(sourcePath, destPath); });
        }

        public void MoveDirectory(string sourcePath, string destPath)
        {
            Directory.Move(sourcePath, destPath);
        }
    }
}