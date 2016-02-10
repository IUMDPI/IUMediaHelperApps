using System;
using System.Text;

namespace Packager.Models.EmailMessageModels
{
    public class EngineIssueMessage : AbstractEmailMessage
    {
        private const string TitleFormat = "Issue occurred while processing objects on workstation {0}";
        private const string HeaderFormat = "<html><body>";
        private const string FooterFormat = "</body></html>";
        private const string Para1Format = "<p>An issue occurred while processing objects on workstation {0}:</p>";
        private const string Para2Format = "<blockquote>{0}</blockquote>";
        private const string Para3Format = "<p>Stack trace:</p>";
        private const string Para4Format = "<p><pre>{0}</pre></p>";

        public EngineIssueMessage(string[] toAddresses, string fromAddress, string[] attachmentPaths, string machineName,
            Exception issue)
            : base(toAddresses, fromAddress, attachmentPaths)
        {
            MachineName = machineName;
            Issue = issue;
        }
        
        private string MachineName { get; }
        private Exception Issue { get; }

        public override string Title => string.Format(TitleFormat, MachineName);

        public override string Body
        {
            get
            {
                var builder = new StringBuilder();
                builder.AppendLine(HeaderFormat);
                builder.AppendFormat(Para1Format, MachineName);
                builder.AppendLine();
                builder.AppendFormat(Para2Format, Issue.Message);
                builder.AppendLine();
                builder.AppendFormat(Para3Format);
                builder.AppendLine();
                builder.AppendFormat(Para4Format, Issue.StackTrace);
                builder.AppendLine();
                builder.AppendLine(GetAttachmentsText());

                builder.AppendLine(FooterFormat);

                return builder.ToString();
            }
        }
    }
}