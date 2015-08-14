using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Packager.Validators
{
    public class ValidationResults : List<ValidationResult>
    {
        public bool Succeeded
        {
            get { return TrueForAll(r => r.Result); }
        }

        public IEnumerable<string> Issues
        {
            get { return this.Where(v => v.Result == false).Where(v => !string.IsNullOrWhiteSpace(v.Issue)).Select(v => v.Issue).ToList(); }
        }

        public void Add(string issue, params object[] args)
        {
            Add(new ValidationResult(issue, args));
        }

        public string GetIssues(string baseMessage)
        {
            var builder = new StringBuilder();
            builder.AppendLine(baseMessage);
            foreach (var issue in Issues)
            {
                builder.AppendFormat("  {0}/n", issue);
            }

            return builder.ToString();
        }
    }
}