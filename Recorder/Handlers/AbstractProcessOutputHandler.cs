using System.Diagnostics;

namespace Recorder.Handlers
{
    public abstract class AbstractProcessOutputHandler
    {
        public abstract void OnDataReceived(object sender, DataReceivedEventArgs args);

        public abstract void Reset();
    }
}