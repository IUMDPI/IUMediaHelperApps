using System;

// Adapted from https://github.com/aelij/RawInputProcessor 
// and http://www.codeproject.com/Articles/17123/Using-Raw-Input-from-C-to-handle-multiple-keyboard
namespace Recorder.BarcodeScanner
{
    public sealed class RawKeyboardDevice
    {
        public string Name { get; private set; }
        public RawDeviceType Type { get; private set; }
        public IntPtr Handle { get; private set; }
        public string Description { get; private set; }

        internal RawKeyboardDevice(string name, RawDeviceType type, IntPtr handle, string description)
        {
            Handle = handle;
            Type = type;
            Name = name;
            Description = description;
        }

        public override string ToString()
        {
            return string.Format("Device\n Name: {0}\n Type: {1}\n Handle: {2}\n Name: {3}\n",
                Name,
                Type,
                Handle.ToInt64().ToString("X"),
                Description);
        }
    }
}