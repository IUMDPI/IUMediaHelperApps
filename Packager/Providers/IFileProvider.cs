using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Threading.Tasks;

namespace Packager.Providers
{
    public interface IFileProvider
    {
        void Move(string sourceFileName, string destFileName);
        void Copy(string sourceFileName, string destFileName);
        void WriteAllText(string path, string contents);

        Task CopyFileAsync(string sourceFileName, string destFileName);
        Task MoveFileAsync(string sourceFileName, string destFileName);
        bool FileExists(string path);
        bool FileDoesNotExist(string path);
        FileInfo GetFileInfo(string path);
        FileVersionInfo GetFileVersionInfo(string path);
        string GetFileVersion(string path);

        Task ArchiveFile(string filePath, string archivePath);
    }

    public class FileProvider : IFileProvider
    {
        public void Move(string sourceFileName, string destFileName)
        {
            File.Move(sourceFileName, destFileName);
        }

        public void Copy(string sourceFileName, string destFileName)
        {
            File.Move(sourceFileName, destFileName);
        }

        public void WriteAllText(string path, string contents)
        {
            File.WriteAllText(path, contents);
        }

        public async Task CopyFileAsync(string sourceFileName, string destFileName)
        {
            await Task.Run(() => { File.Copy(sourceFileName, destFileName); });
        }

        public async Task MoveFileAsync(string sourceFileName, string destFileName)
        {
            await Task.Run(() => { File.Move(sourceFileName, destFileName); });
        }

        public bool FileExists(string path)
        {
            return File.Exists(path);
        }

        public bool FileDoesNotExist(string path)
        {
            return FileExists(path) ==false;
        }

        public FileInfo GetFileInfo(string path)
        {
            return new FileInfo(path);
        }

        public FileVersionInfo GetFileVersionInfo(string path)
        {
            return !FileExists(path) 
                ? null 
                : FileVersionInfo.GetVersionInfo(path);
        }

        public string GetFileVersion(string path)
        {
            var info = GetFileVersionInfo(path);
            return info == null 
                ? "" 
                : info.FileVersion;
        }

        private void ArchiveFileInternal(string filePath, string archivePath)
        {
            if (FileDoesNotExist(filePath))
            {
                throw new FileNotFoundException($"The file {filePath} does not exist or is not accessible");
            }

            var mode = FileExists(archivePath) ? ZipArchiveMode.Update : ZipArchiveMode.Create;
            using (var archive = ZipFile.Open(archivePath, mode))
            {
                archive.CreateEntryFromFile(filePath, Path.GetFileName(filePath));
            }
        }

        public async Task ArchiveFile(string filePath, string archivePath)
        {
            await Task.Run(() => ArchiveFileInternal(filePath, archivePath));
        }
    }
}