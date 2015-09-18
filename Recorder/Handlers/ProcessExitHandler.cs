using System;
using Recorder.Utilities;

namespace Recorder.Handlers
{
    public class ProcessExitHandler : AbstractRecordingHandler
    {
        public ProcessExitHandler(RecordingEngine recorder) : base(recorder)
        {
        }

        public void OnExitHandler(object sender, EventArgs args)
        {
            Recorder.Recording = false;
        }
    }
}