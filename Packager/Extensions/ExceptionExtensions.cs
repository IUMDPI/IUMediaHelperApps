using System;

namespace Packager.Extensions
{
    public static class ExceptionExtensions
    {
        public static string GetBaseMessage(this Exception exception)
        {
            return exception?.GetBaseException().Message ?? "";
        }
    }
}