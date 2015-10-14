using System;

namespace Recorder.Exceptions
{
    public class AbstractHandledException : Exception
    {
        public AbstractHandledException()
        {
        }

        public AbstractHandledException(string message) : base(message)
        {
        }

        public AbstractHandledException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}