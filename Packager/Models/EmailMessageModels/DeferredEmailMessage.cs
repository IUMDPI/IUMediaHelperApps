using System.Collections.Generic;
using System.Linq;
using System.Text;
using Packager.Extensions;
using Packager.Models.ResultModels;
using Packager.Verifiers;

namespace Packager.Models.EmailMessageModels
{
    public class DeferredEmailMessage:AbstractEmailMessage
    {
        private IEnumerable<KeyValuePair<string, DurationResult>> Results { get; }
        private string[] Barcodes => Results.Select(r => r.Key).ToArray();
        private string MachineName { get; }
        private const string HeaderFormat = "<html><body>";
        private const string FooterFormat = "</body></html>";
        private const string Para1Format = "<p>The packager has deferred {0} on workstation {1}:</p>";
        
        public DeferredEmailMessage(IEnumerable<KeyValuePair<string, DurationResult>> results, string[] toAddresses, string fromAddress, string machineName, string log) : base(toAddresses, fromAddress, new [] {log})
        {
            Results = results;
            MachineName = machineName;
        }

        public override string Title=> $"{Barcodes.ToSingularOrPlural("object", "objects")} deferred on workstation {MachineName}";

        public override string Body
        {
            get
            {
                var builder = new StringBuilder();
                builder.AppendLine(HeaderFormat);
                builder.AppendFormat(Para1Format, Barcodes.ToSingularOrPlural("object", "objects"), MachineName);
                builder.AppendLine();
                builder.AppendLine("<ul>");
                foreach (var result in Results)
                {
                    builder.AppendLine($"<li>{result.Key} -- {result.Value.Issue.ToDefaultIfEmpty().ToLowerInvariant()}</li>");
                }
                builder.AppendLine("</ul>");

                if (Attachments.Any())
                {
                    builder.AppendLine(
                        "<p>See the attached log file for more details.</p>");
                }

                builder.AppendLine(FooterFormat);



                return builder.ToString();
            } 
        }
    }
}
