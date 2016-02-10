using System;
using System.Text;
using Packager.Providers;

namespace Packager.Utilities.Process
{
    public class FileOutputBuffer : IOutputBuffer, IDisposable
    {
        private readonly string _path;
        private readonly IFileProvider _fileProvider;

        private readonly StringBuilder _buffer = new StringBuilder();

        public FileOutputBuffer(string path, IFileProvider fileProvider)
        {
            _path = path;
            _fileProvider = fileProvider;
        }

        public void AppendLine(string value)
        {
            _buffer.AppendLine(value);
            if (_buffer.Length < 50000)
            {
                return;
            }

            Flush();
        }

        public string GetContent()
        {
            return _fileProvider.ReadAllText(_path);
        }

        private void Flush()
        {
            _fileProvider.AppendText(_path, _buffer.ToString());
            _buffer.Clear();
        }

        public void Dispose()
        {
            Flush();
        }
    }
}