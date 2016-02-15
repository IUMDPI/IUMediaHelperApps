using System;
using System.IO;
using System.Linq;
using Packager.Exceptions;
using Packager.Extensions;
using Packager.Models.EmailMessageModels;
using Packager.Models.SettingsModels;
using Packager.Providers;
using Packager.Utilities.Email;

namespace Packager.Observers
{
    public class IssueEmailerObserver : IObserver
    {
        public IssueEmailerObserver(IProgramSettings programSettings, ISystemInfoProvider systemInfo,
            IEmailSender emailSender)
        {
            ProgramSettings = programSettings;
            SystemInfo = systemInfo;
            EmailSender = emailSender;
        }

        private IProgramSettings ProgramSettings { get; }
        private ISystemInfoProvider SystemInfo { get; }
        private IEmailSender EmailSender { get; }

        public void Log(string baseMessage, params object[] elements)
        {
            // ignore
        }

        public void LogProcessingError(Exception issue, string barcode)
        {
            if (ShouldSendMessage(issue) == false)
            {
                return;
            }

            var message = new ProcessingIssueMessage(
                ProgramSettings.IssueNotifyEmailAddresses,
                ProgramSettings.FromEmailAddress,
                GetProcessingIssueAttachments(barcode),
                barcode,
                SystemInfo.MachineName,
                issue);

            EmailSender.Send(message);
        }

        public void LogEngineError(Exception issue)
        {
            if (ShouldSendMessage(issue) == false)
            {
                return;
            }

            var message = new EngineIssueMessage(
                ProgramSettings.IssueNotifyEmailAddresses,
                ProgramSettings.FromEmailAddress,
                new[] {SystemInfo.CurrentSystemLogPath},
                SystemInfo.MachineName,
                issue);

            EmailSender.Send(message);
        }

        public void BeginSection(string sectionKey, string baseMessage, params object[] elements)
        {
            // ignore
        }

        public void EndSection(string sectionKey, string newTitle = "", bool collapse = false)
        {
            // ignore
        }

        public int UniqueIdentifier => 4;

        private string[] GetProcessingIssueAttachments(string barcode)
        {
            var objectLogPath = Path.Combine(
                ProgramSettings.LogDirectoryName,
                $"{ProgramSettings.ProjectCode}_{barcode}.log");

            return new[] {SystemInfo.CurrentSystemLogPath, objectLogPath};
        }

        private bool ShouldSendMessage(Exception issue)
        {
            if (ProgramSettings.SmtpServer.IsNotSet())
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