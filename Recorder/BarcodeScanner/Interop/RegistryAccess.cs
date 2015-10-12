using Microsoft.Win32;

// Adapted from https://github.com/aelij/RawInputProcessor 
// and http://www.codeproject.com/Articles/17123/Using-Raw-Input-from-C-to-handle-multiple-keyboard
namespace Recorder.BarcodeScanner.Interop
{
    internal static class RegistryAccess
    {
        private const string Prefix = @"\\?\";

        internal static RegistryKey GetDeviceKey(string device)
        {
            if (device == null || !device.StartsWith(Prefix)) return null;
            var array = device.Substring(Prefix.Length).Split('#');
            return array.Length < 3 
                ? null 
                : Registry.LocalMachine.OpenSubKey($@"System\CurrentControlSet\Enum\{array[0]}\{array[1]}\{array[2]}");
        }

        internal static string GetClassType(string classGuid)
        {
            var registryKey =
                Registry.LocalMachine.OpenSubKey("SYSTEM\\CurrentControlSet\\Control\\Class\\" + classGuid);
            if (registryKey == null)
            {
                return string.Empty;
            }
            return (string) registryKey.GetValue("Class");
        }
    }
}