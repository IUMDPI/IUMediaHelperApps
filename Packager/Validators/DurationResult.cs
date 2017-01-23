using System;

namespace Packager.Validators
{
    public class DurationResult : ValidationResult
    {
        public TimeSpan Duration { get; private set; }
        public DateTime Timestamp { get; }

        public DurationResult(DateTime startTime, string baseIssue, params object[] args) : this(startTime, false, string.Format(baseIssue, args))
        {

        }

        private DurationResult(DateTime startTime, bool success, string issue) : base(success, issue)
        {
            Timestamp = startTime;
            Duration = DateTime.Now - startTime;
        }

        public new static DurationResult Success(DateTime startTime) =>
            new DurationResult(startTime, true, "");
    }
}