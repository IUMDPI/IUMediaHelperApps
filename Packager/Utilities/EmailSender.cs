using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Net.Mime;
using Packager.Models.EmailMessageModels;
using Packager.Providers;

namespace Packager.Utilities
{
    public class EmailSender : IEmailSender
    {
        public EmailSender(IFileProvider fileProvider, string smtpServer)
        {
            FileProvider = fileProvider;
            SmtpServer = smtpServer;
        }

        private IFileProvider FileProvider { get; set; }
        private string SmtpServer { get; set; }

        public void Send(AbstractEmailMessage message)
        {
            try
            {
                using (var client = new SmtpClient(SmtpServer){UseDefaultCredentials = true, EnableSsl = true})
                {
                    foreach (var emailMessage in message.ToAddresses.Select(address => GetMessage(message, address)))
                    {
                        client.Send(emailMessage);
                    }
                }
            }
            catch (Exception e)
            {
                // ignore
            }
        }

        private MailMessage GetMessage(AbstractEmailMessage message, string address)
        {
            var result = new MailMessage(message.From, address, message.Title, message.Body) {IsBodyHtml = true};

            foreach (var attachment in GetAttachments(message))
            {
                result.Attachments.Add(attachment);
            }

            return result;
        }

        private IEnumerable<Attachment> GetAttachments(AbstractEmailMessage message)
        {
            if (message.Attachments == null || !message.Attachments.Any())
            {
                return new List<Attachment>();
            }

            var result = new List<Attachment>();
            foreach (var filePath in message.Attachments)
            {
                if (!FileProvider.FileExists(filePath))
                {
                    continue;
                }
                
                var attachment = new Attachment(filePath, MediaTypeNames.Application.Octet);

                var info = FileProvider.GetFileInfo(filePath);
                attachment.ContentDisposition.CreationDate = info.CreationTimeUtc;
                attachment.ContentDisposition.ModificationDate = info.LastWriteTimeUtc;
                attachment.ContentDisposition.ReadDate = info.LastAccessTimeUtc;
                attachment.ContentDisposition.FileName = info.Name;
                attachment.ContentDisposition.Size = info.Length;
                attachment.ContentDisposition.DispositionType = DispositionTypeNames.Attachment;
                result.Add(attachment);
            }

            return result;
        }
    }
}