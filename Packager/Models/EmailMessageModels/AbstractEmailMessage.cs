using System.IO;
using System.Linq;

namespace Packager.Models.EmailMessageModels
{
    public abstract class AbstractEmailMessage
    {
        private const string AttachmentFormat = "<p>For more details, see the attached application {0}: {1}</p>";

        protected AbstractEmailMessage(string[] toAddresses, string fromAddress, string[] attachments)
        {
            ToAddresses = toAddresses;
            From = fromAddress;
            Attachments = attachments;
        }

        public string[] ToAddresses { get; set; }
        public string From { get; set; }
        public abstract string Title { get; }
        public abstract string Body { get; }
        public string[] Attachments { get; set; }

        protected string GetAttachmentsText()
        {
            if (!Attachments.Any()) return "";
            return Attachments.Count() == 1
                ? string.Format(AttachmentFormat, "log", Path.GetFileName(Attachments.Single()))
                : string.Format(AttachmentFormat, "logs", string.Join(", ", Attachments.Select(Path.GetFileName)));
        }
    }
}