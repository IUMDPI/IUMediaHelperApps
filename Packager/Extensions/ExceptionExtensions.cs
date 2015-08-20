using System;

namespace Packager.Extensions
{
    public static class ExceptionExtensions
    {
        public static string GetFirstMessage(this Exception exception)
        {
            if (exception == null)
            {
                return "";
            }

            return string.IsNullOrWhiteSpace(exception.Message) 
                ? exception.InnerException.GetFirstMessage() 
                : exception.Message;
        }
    }
}