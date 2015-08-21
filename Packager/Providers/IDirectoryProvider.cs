using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Packager.Providers
{
    public interface IDirectoryProvider
    {
        IEnumerable<string> EnumerateFiles(string path);
        DirectoryInfo CreateDirectory(string path);
        Task MoveDirectoryAsync(string sourcePath, string destPath);
        void MoveDirectory(string sourcePath, string destPath);
        bool DirectoryExists(string value);

        Task DeleteDirectoryAsync(string path);

        IEnumerable<KeyValuePair<string, DateTime>> GetFolderNamesAndCreationDates(string parent);
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

        public bool DirectoryExists(string value)
        {
            return Directory.Exists(value);
        }

        public async Task DeleteDirectoryAsync(string path)
        {
            if (!DirectoryExists(path))
            {
                return;
            }

            await Task.Run(() => { Directory.Delete(path,true); });
        }

        public IEnumerable<KeyValuePair<string, DateTime>> GetFolderNamesAndCreationDates(string parent)
        {
            return Directory.EnumerateDirectories(parent)
                .Select(directory => new DirectoryInfo(directory))
                .Select(i=>new KeyValuePair<string, DateTime>(i.Name, i.CreationTime))
                .ToList();
        }
    }
}