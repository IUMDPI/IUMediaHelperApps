using System;

namespace Recorder.Exceptions
{
    public class RecordingException : AbstractHandledException
    {
        public RecordingException()
        {
        }

        public RecordingException(string message) : base(message)
        {
        }

        public RecordingException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}