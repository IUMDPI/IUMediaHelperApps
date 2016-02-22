using System.Text;

namespace Packager.Utilities.ProcessRunners
{
    public class StringOutputBuffer : IOutputBuffer
    {
        private readonly StringBuilder _buffer = new StringBuilder();

        public void AppendLine(string value)
        {
            _buffer.AppendLine(value);
        }

        public string GetContent()
        {
            return _buffer.ToString();
        }
    }
}