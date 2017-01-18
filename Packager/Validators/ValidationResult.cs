using System;

namespace Packager.Validators
{
    public class ValidationResult
    {
        public ValidationResult(string baseIssue, params object[] args)
            : this(false, string.Format(baseIssue, args))
        {
            Timestamp = DateTime.UtcNow;
        }

        private ValidationResult(bool success, string issue)
        {
            Issue = issue;
            Result = success;
            Timestamp = DateTime.UtcNow;
        }

        public static ValidationResult Success => new ValidationResult(true, "");

        public bool Result { get; set; }
        public string Issue { get; set; }
        public DateTime Timestamp { get; }
    }
}