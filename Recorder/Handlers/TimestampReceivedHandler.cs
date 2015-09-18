using System;
using System.Diagnostics;
using System.Text.RegularExpressions;
using Recorder.Utilities;

namespace Recorder.Handlers
{
    public class TimestampReceivedHandler:AbstractRecordingHandler
    {
        private readonly Regex _timestampExpression = new Regex(@"time=(\d{2}:\d{2}:\d{2}\.\d{2})");

        public delegate void TimestampDelegate(TimeSpan timeSpan);

        public TimestampReceivedHandler(RecordingEngine recorder) : base(recorder)
        {
         
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