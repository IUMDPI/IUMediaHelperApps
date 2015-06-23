using System;

namespace Packager.Exceptions
{
    public class LoggedException : Exception
    {
        public LoggedException(Exception innerException) : base("", innerException)
        {
        }
    }
}