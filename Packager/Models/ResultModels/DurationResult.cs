using System;

namespace Packager.Models.ResultModels
{
    public class DurationResult
    {
        public TimeSpan Duration { get; }
        public DateTime Timestamp { get; }
        public bool Succeeded  { get; }
        public bool Failed { get; }
        public bool Skipped { get; }
        public string Issue { get; }

        public DurationResult(DateTime startTime, string baseIssue, params object[] args) 
            : this(startTime, false, true, false, string.Format(baseIssue, args))
        {
            
        }

        private DurationResult(DateTime startTime, bool succeeded, bool failed, bool deferred, string issue) 
        {
            Timestamp = startTime;
            Duration = DateTime.Now - startTime;
            Succeeded = succeeded;
            Failed = failed;
            Skipped = deferred;
            Issue = issue;
            
        }

        public static DurationResult Success(DateTime startTime) =>
            new DurationResult(startTime, true, false, false, "");

        public static DurationResult Deferred (DateTime startTime, string baseIssue, params object[] args) => 
            new DurationResult(startTime, false, false, true, string.Format(baseIssue, args));
    }
}