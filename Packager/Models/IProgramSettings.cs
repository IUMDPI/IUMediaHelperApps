using System.Collections.Generic;

namespace Packager.Models
{
    public interface IProgramSettings
    {
        // ReSharper disable once InconsistentNaming
        string BwfMetaEditPath { get; }
        // ReSharper disable once InconsistentNaming
        string FFMPEGPath { get; }
        string FFProbePath { get; }
        string InputDirectory { get; }
        string ProcessingDirectory { get; }
        string FFMPEGAudioProductionArguments { get; }
        string FFMPEGAudioAccessArguments { get; }
        string FFMPEGVideoMezzanineArguments { get; }
        string FFMPEGVideoAccessArguments { get; }
        string ProjectCode { get; }
        string DropBoxDirectoryName { get; }
        PodAuth PodAuth { get; }
        string WebServiceUrl { get; }
        string ErrorDirectoryName { get; }
        string SuccessDirectoryName { get; }
        string LogDirectoryName { get; }
        string[] IssueNotifyEmailAddresses { get; }
        string SmtpServer { get; }
        string FromEmailAddress { get; }
        int DeleteSuccessfulObjectsAfterDays { get; }
        List<string> Issues { get; }
        string UnitPrefix { get; }
    }
}