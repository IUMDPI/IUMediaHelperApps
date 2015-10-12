using System;
using System.Windows.Input;

// Adapted from https://github.com/aelij/RawInputProcessor 
// and http://www.codeproject.com/Articles/17123/Using-Raw-Input-from-C-to-handle-multiple-keyboard
namespace Recorder.BarcodeScanner
{
    public sealed class RawInputEventArgs : EventArgs
    {
        public RawKeyboardDevice Device { get; private set; }
        public KeyPressState KeyPressState { get; private set; }
        public uint Message { get; private set; }
        public Key Key { get; private set; }
        public int VirtualKey { get; private set; }
        public bool Handled { get; set; }

        internal RawInputEventArgs(RawKeyboardDevice device, KeyPressState keyPressState, uint message, Key key,
            int virtualKey)
        {
            Device = device;
            KeyPressState = keyPressState;
            Message = message;
            Key = key;
            VirtualKey = virtualKey;
        }
    }
}