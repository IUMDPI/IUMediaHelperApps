using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Recorder.Utilities;

namespace Recorder.Handlers
{
    public class AbstractRecordingHandler
    {
        protected RecordingEngine Recorder { get; set; }

        public AbstractRecordingHandler(RecordingEngine recorder)
        {
            Recorder = recorder;
        }
    }
}
