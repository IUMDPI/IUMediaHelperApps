using System;
using System.Linq;
using Packager.Exceptions;
using Packager.Models;
using Packager.Models.EmailMessageModels;
using Packager.Providers;
using Packager.Utilities;

namespace Packager.Observers
{
    public class IssueEmailerObserver : IObserver
    {
        public IssueEmailerObserver(IProgramSettings programSettings, ISystemInfoProvider systemInfo, IEmailSender emailSender)
        {
            ProgramSettings = programSettings;
            SystemInfo = systemInfo;
            EmailSender = emailSender;
        }

        private IProgramSettings ProgramSettings { get; }
        private ISystemInfoProvider SystemInfo { get; set; }
        private IEmailSender EmailSender { get; set; }

        public void Log(string baseMessage, params object[] elements)
        {
            // ignore
        }

        public void LogProcessingError(Exception issue, string barcode)
        {
            if (!ShouldSendMessage(issue))
            {
            }

              var message = new ProcessingIssueMessage(
                ProgramSettings.IssueNotifyEmailAddresses,
                ProgramSettings.FromEmailAddress,
                new[]{SystemInfo.CurrentLogPath}, 
                barcode, 
                SystemInfo.MachineName, 
                issue);

            EmailSender.Send(message);
        }

        public void LogEngineError(Exception issue)
        {
            if (!ShouldSendMessage(issue))
            {
            }

              var message = new EngineIssueMessage(
                ProgramSettings.IssueNotifyEmailAddresses,
                ProgramSettings.FromEmailAddress,
                new[] { SystemInfo.CurrentLogPath }, 
                SystemInfo.MachineName, 
                issue);

            EmailSender.Send(message);
        }

        private bool ShouldSendMessage(Exception issue)
        {
            if (string.IsNullOrWhiteSpace(ProgramSettings.SmtpServer))
            {
                return false;
            }

            if (issue is LoggedException)
            {
                return false;
            }

            return ProgramSettings.IssueNotifyEmailAddresses.Any();
        }
    }
}