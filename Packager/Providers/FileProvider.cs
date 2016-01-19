using System;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Packager.Models.SettingsModels;

namespace Packager.Providers
{
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

        public void AppendText(string path, string text)
        {
            using (var writer = File.AppendText(path))
            {
                writer.WriteLine(text);
            }
        }

        public string ReadAllText(string path)
        {
            return File.ReadAllText(path);
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

        public T Deserialize<T>(string filePath)
        {
            try
            {
                var serializer = new XmlSerializer(typeof(T));
                using (var stream = new FileStream(filePath, FileMode.Open))
                {
                    return (T)serializer.Deserialize(stream);
                }
            }
            catch (Exception)
            {
                return default(T);
            }
        }
    }
}