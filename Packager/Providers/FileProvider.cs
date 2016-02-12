using System;
using System.IO;
using System.IO.Compression;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Packager.Providers
{
    public class FileProvider : IFileProvider
    {
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
            return FileExists(path) == false;
        }

        public FileInfo GetFileInfo(string path)
        {
            return new FileInfo(path);
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

        public async Task ArchiveFile(string filePath, string archivePath)
        {
            await ArchiveFileInternal(filePath, archivePath);
        }

        public T Deserialize<T>(string filePath)
        {
            try
            {
                var serializer = new XmlSerializer(typeof (T));
                using (var stream = new FileStream(filePath, FileMode.Open))
                {
                    return (T) serializer.Deserialize(stream);
                }
            }
            catch (Exception)
            {
                return default(T);
            }
        }

        private async Task ArchiveFileInternal(string filePath, string archivePath)
        {
            if (FileDoesNotExist(filePath))
            {
                throw new FileNotFoundException($"The file {filePath} does not exist or is not accessible");
            }

            var info = new FileInfo(filePath);
            using (var inputStream = info.OpenRead())
            {
                using (var outputStream = File.Create(archivePath))
                {
                    using (var archiveStream = new GZipStream(outputStream, CompressionMode.Compress))
                    {
                        await inputStream.CopyToAsync(archiveStream);
                    }
                }
            }
        }
    }
}