using System;

namespace Packager.Extensions
{
    public static class ExceptionExtensions
    {
        public static string GetBaseMessage(this Exception exception)
        {
            if (exception == null)
            {
                return "";
            }

            return exception.GetBaseException().Message;

          
        }
    }
}