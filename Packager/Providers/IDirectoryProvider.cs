using System;
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
        bool DirectoryExists(string value);

        Task DeleteDirectoryAsync(string path);

        IEnumerable<KeyValuePair<string, DateTime>> GetFolderNamesAndCreationDates(string parent);
    }
}