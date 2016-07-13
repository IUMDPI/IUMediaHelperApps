using System.Windows.Forms.VisualStyles;

namespace Packager.Models.SettingsModels
{
    public interface IProgramSettings
    {
        string BwfMetaEditPath { get; }
        string FFMPEGPath { get; }
        string FFProbePath { get; }
        string InputDirectory { get; }
        string ProcessingDirectory { get; }
        string FFMPEGAudioProductionArguments { get; }
        string FFMPEGAudioAccessArguments { get; }
        string FFMPEGVideoMezzanineArguments { get; }
        string FFMPEGVideoAccessArguments { get; }
        string FFProbeVideoQualityControlArguments { get; }
        string ProjectCode { get; }
        string DropBoxDirectoryName { get; }
        string WebServiceUrl { get; }
        string ErrorDirectoryName { get; }
        string SuccessDirectoryName { get; }
        string LogDirectoryName { get; }
        string[] IssueNotifyEmailAddresses { get; }
        string[] SuccessNotifyEmailAddresses { get; }
        string SmtpServer { get; }
        string FromEmailAddress { get; }
        int DeleteSuccessfulObjectsAfterDays { get; }
        string UnitPrefix { get; }
        string PodAuthFilePath { get; }
        string DigitizingEntity { get; }
    }
}