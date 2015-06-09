using System.IO;
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
        FileInfo GetFileInfo(string path);
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

        public FileInfo GetFileInfo(string path)
        {
            return new FileInfo(path);
        }
    }
}