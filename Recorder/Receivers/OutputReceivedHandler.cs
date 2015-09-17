using System;
using System.Diagnostics;
using System.Text;
using System.Text.RegularExpressions;

namespace Recorder.Receivers
{
    public class OutputReceivedHandler
    {
        private readonly TimestampDelegate _timestampDelegate;

        private readonly Regex _timestampExpression = new Regex(@"time=(\d{2}:\d{2}:\d{2}\.\d{2})");

        public delegate void TimestampDelegate(TimeSpan timeSpan);

        public OutputReceivedHandler(TimestampDelegate @delegate)
        {
            _timestampDelegate = @delegate;
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

            _timestampDelegate(timeSpan);
        }
    }
}