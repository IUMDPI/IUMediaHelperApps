using Packager.Utilities;

namespace Packager.Models
{
    public interface IProgramSettings
    {
        // ReSharper disable once InconsistentNaming
        string BwfMetaEditPath { get; }
        // ReSharper disable once InconsistentNaming
        string FFMPEGPath { get; }
        string InputDirectory { get; }
        string ProcessingDirectory { get; }
        // ReSharper disable once InconsistentNaming
        string FFMPEGAudioProductionArguments { get; }
        // ReSharper disable once InconsistentNaming
        string FFMPEGAudioAccessArguments { get; }
        string ProjectCode { get; }
        string DropBoxDirectoryName { get; }
        string DateFormat { get; }
        PodAuth PodAuth { get; }
        string WebServiceUrl { get; }
        string ErrorDirectoryName { get; }
        string SuccessDirectoryName { get; }
        string LogDirectoryName { get; }
        string[] IssueNotifyEmailAddresses { get; }
        string SmtpServer { get; }
        string FromEmailAddress { get; }
        int DeleteSuccessfulObjectsAfterDays { get; }

        BextFields[] SuppressAudioMetadataFields { get; }
    }
}