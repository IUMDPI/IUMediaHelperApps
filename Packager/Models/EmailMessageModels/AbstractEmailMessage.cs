using System.IO;
using System.Linq;

namespace Packager.Models.EmailMessageModels
{
    public abstract class AbstractEmailMessage
    {
        private const string AttachmentFormat = "<p>For more details, see the attached {0}: {1}</p>";

        protected AbstractEmailMessage(string[] toAddresses, string fromAddress, string[] attachments)
        {
            ToAddresses = toAddresses;
            From = fromAddress;
            Attachments = attachments;
        }

        public string[] ToAddresses { get; }
        public string From { get; }
        public abstract string Title { get; }
        public abstract string Body { get; }
        public string[] Attachments { get; }

        protected string GetAttachmentsText()
        {
            if (!Attachments.Any()) return "";

            return Attachments.Length == 1
                ? string.Format(AttachmentFormat, "log", Path.GetFileName(Attachments.Single()))
                : string.Format(AttachmentFormat, "logs", string.Join(", ", Attachments.Select(Path.GetFileName)));
        }
    }
}