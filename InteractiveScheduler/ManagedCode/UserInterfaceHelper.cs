using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Media.Imaging;

namespace InteractiveScheduler.ManagedCode
{
    public static class UserInterfaceHelper
    {
        private const int MaxPath = 260;
        private const uint ShieldIconId = 77;
        private const uint ShieldIconFlags = 0x000000100 | 0x000000001;

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        private struct StockIconInfo
        {
            public uint Size;
            public readonly IntPtr IconHandle;
            private readonly int SysIconIndex;
            private readonly int Icon;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = MaxPath)]
            private readonly string Path;
        }

        [DllImport("Shell32.dll", SetLastError = false)]
        private static extern int SHGetStockIconInfo(uint siid, uint uFlags, ref StockIconInfo psii);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool DestroyIcon(IntPtr hIcon);
        
        public static BitmapSource GetUserAccessControlShield()
        {
            if (Environment.OSVersion.Version.Major < 6)
            {
                return System.Windows.Interop.Imaging.CreateBitmapSourceFromHIcon(
                    SystemIcons.Shield.Handle,
                    Int32Rect.Empty,
                    BitmapSizeOptions.FromEmptyOptions());
            }

            var info = new StockIconInfo { Size = (uint)Marshal.SizeOf(typeof(StockIconInfo)) };
            try
            {
                Marshal.ThrowExceptionForHR(SHGetStockIconInfo(ShieldIconId, ShieldIconFlags, ref info));

                using (var icon = Icon.FromHandle(info.IconHandle))
                {
                    return System.Windows.Interop.Imaging.CreateBitmapSourceFromHIcon(
                        icon.Handle,
                        new Int32Rect(0, 0, 16, 16),
                        BitmapSizeOptions.FromEmptyOptions());
                }
                
            }
            finally
            {
                DestroyIcon(info.IconHandle);
            }
        }
    }
}
