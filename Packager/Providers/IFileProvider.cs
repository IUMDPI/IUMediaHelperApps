using System.IO;
using System.Runtime.CompilerServices;

namespace Packager.Providers
{
    public interface IFileProvider
    {
        void Move(string sourceFileName, string destFileName);
        void Copy(string sourceFileName, string destFileName);
        void WriteAllText(string path, string contents);
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

        public FileInfo GetFileInfo(string path)
        {
            return new FileInfo(path);
        }
    }
}