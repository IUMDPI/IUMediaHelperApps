using System;

namespace Packager.Models.ResultModels
{
    public class BooleanReason
    {
        private BooleanReason(bool succeeded, Exception issue)
        {
            Succeeded = succeeded;
            Reason = issue;
        }

        public BooleanReason(Exception issue) : this(false, issue)
        {
        }

        public bool Succeeded { get; set; }
        public Exception Reason { get; set; }

        public static BooleanReason Success()
        {
            return new BooleanReason(true, null);
        }
    }
}