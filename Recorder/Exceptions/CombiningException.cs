using System;

namespace Recorder.Exceptions
{
    public class CombiningException : AbstractHandledException
    {
        public CombiningException()
        {
        }

        public CombiningException(string message) : base(message)
        {
        }

        public CombiningException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}