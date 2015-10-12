using System;
using System.Diagnostics;
using System.Text.RegularExpressions;
using Recorder.Utilities;

namespace Recorder.Handlers
{
    public class TimestampReceivedHandler
    {
        private RecordingEngine Recorder { get; }
        private readonly Regex _timestampExpression = new Regex(@"time=(\d{2}:\d{2}:\d{2}\.\d{2})");
        
        public TimestampReceivedHandler(RecordingEngine recorder)
        {
            Recorder = recorder;
        }

        public void Reset()
        {
            Recorder.OnTimestampUpdated(new TimeSpan());
        }

        public void OnDataReceived(object sender, DataReceivedEventArgs args)
        {
            if (string.IsNullOrWhiteSpace(args.Data))
            {
                return;
            }
            
            var match = _timestampExpression.Match(args.Data);
            if (match.Success == false)
            {
                return;
            }

            TimeSpan timeSpan;
            if (!TimeSpan.TryParse(match.Groups[1].ToString(), out timeSpan))
            {
                return;
            }

            Recorder.OnTimestampUpdated(timeSpan);
        }
    }
}