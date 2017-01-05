using System;
using System.Runtime.InteropServices;
using System.Security;

namespace InteractiveScheduler.Extensions
{
    public static class SecureStringExtensions
    {
        public static string ToUnsecureString(this SecureString password)
        {
            if (password == null)
                throw new ArgumentNullException(nameof(password));

            var unmanagedString = IntPtr.Zero;
            try
            {
                unmanagedString = Marshal.SecureStringToGlobalAllocUnicode(password);
                return Marshal.PtrToStringUni(unmanagedString);
            }
            finally
            {
                Marshal.ZeroFreeGlobalAllocUnicode(unmanagedString);
            }
        }
    }
}
