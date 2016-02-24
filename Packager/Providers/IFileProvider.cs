using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Packager.Providers
{
    public interface IFileProvider
    {
        Task CopyFileAsync(string sourceFileName, string destFileName, CancellationToken cancellationToken);
        Task MoveFileAsync(string sourceFileName, string destFileName, CancellationToken cancellationToken);
        bool FileExists(string path);
        bool FileDoesNotExist(string path);
        FileInfo GetFileInfo(string path);
        void AppendText(string path, string text);
        string ReadAllText(string path);
        Task ArchiveFile(string filePath, string archivePath, CancellationToken cancellationToken);
        T Deserialize<T>(string filePath);
    }
}