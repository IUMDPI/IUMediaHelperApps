namespace Packager.Validators
{
    public class ValidationResult
    {
        public ValidationResult(string baseIssue, params object[] args)
            : this(false, string.Format(baseIssue, args))
        {

        }

        protected ValidationResult(bool success, string issue)
        {
            Issue = issue;
            Result = success;
        }

        public static ValidationResult Success => new ValidationResult(true, "");

        public bool Result { get; set; }
        public string Issue { get; set; }

    }
}