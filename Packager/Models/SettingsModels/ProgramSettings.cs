using System.Collections.Generic;
using System.Collections.Specialized;
using Packager.Factories;
using Packager.Validators.Attributes;

namespace Packager.Models.SettingsModels
{
    public class ProgramSettings : IProgramSettings
    {
        public void Import(NameValueCollection dictionary, ISettingsFactory factory)
        {
            BwfMetaEditPath = factory.GetStringValue(dictionary, "PathToMetaEdit");
            FFMPEGPath = factory.GetStringValue(dictionary, "PathToFFMpeg");
            FFProbePath = factory.GetStringValue(dictionary, "PathToFFProbe");
            InputDirectory = factory.GetStringValue(dictionary, "WhereStaffWorkDirectoryName");
            ProcessingDirectory = factory.GetStringValue(dictionary, "ProcessingDirectoryName");
            FFMPEGAudioProductionArguments = factory.GetStringValue(dictionary, "ffmpegAudioProductionArguments");
            FFMPEGAudioAccessArguments = factory.GetStringValue(dictionary, "ffmpegAudioAccessArguments");
            FFMPEGVideoMezzanineArguments = factory.GetStringValue(dictionary, "ffmpegVideoMezzanineArguments");
            FFMPEGVideoAccessArguments = factory.GetStringValue(dictionary, "ffmpegVideoAccessArguments");
            FFProbeVideoQualityControlArguments = factory.GetStringValue(dictionary,
                "ffprobeVideoQualityControlArguments");
            ProjectCode = factory.GetStringValue(dictionary, "ProjectCode");
            DropBoxDirectoryName = factory.GetStringValue(dictionary, "DropBoxDirectoryName");
            ErrorDirectoryName = factory.GetStringValue(dictionary, "ErrorDirectoryName");
            SuccessDirectoryName = factory.GetStringValue(dictionary, "SuccessDirectoryName");
            LogDirectoryName = factory.GetStringValue(dictionary, "LogDirectoryName");

            UnitPrefix = factory.GetStringValue(dictionary, "UnitPrefix");
            SmtpServer = factory.GetStringValue(dictionary, "SmtpServer");
            WebServiceUrl = factory.GetStringValue(dictionary, "WebServiceUrl");
            PodAuthFilePath = factory.GetStringValue(dictionary, "PodAuthorizationFile");
            FromEmailAddress = factory.GetStringValue(dictionary, "FromEmailAddress");

            IssueNotifyEmailAddresses = factory.GetStringValues(dictionary, "IssueNotifyEmailAddresses");
            DeleteSuccessfulObjectsAfterDays = factory.GetIntValue(dictionary, "DeleteProcessedAfterInDays", 0);
        }

        [ValidateFile]
        public string PodAuthFilePath { get; private set; }

        [ValidateFile]
        public string BwfMetaEditPath { get; private set; }

        [ValidateFile]
        public string FFMPEGPath { get; private set; }

        [ValidateFile]
        public string FFProbePath { get; private set; }

        [ValidateFolder]
        public string InputDirectory { get; private set; }

        [ValidateFolder]
        public string ProcessingDirectory { get; private set; }

        [Required]
        public string FFMPEGAudioProductionArguments { get; private set; }

        [Required]
        public string FFMPEGAudioAccessArguments { get; private set; }

        [Required]
        public string FFMPEGVideoMezzanineArguments { get; private set; }

        [Required]
        public string FFMPEGVideoAccessArguments { get; private set; }

        [Required]
        public string FFProbeVideoQualityControlArguments { get; private set; }

        [Required]
        public string ProjectCode { get; private set; }

        [ValidateFolder]
        public string DropBoxDirectoryName { get; private set; }

        [ValidateFolder]
        public string ErrorDirectoryName { get; private set; }

        [ValidateFolder]
        public string SuccessDirectoryName { get; private set; }

        [ValidateFolder]
        public string LogDirectoryName { get; private set; }

        public string[] IssueNotifyEmailAddresses { get; private set; }
        
        public string UnitPrefix { get; private set; }

        public string SmtpServer { get; private set; }

        public string FromEmailAddress { get; private set; }

        public int DeleteSuccessfulObjectsAfterDays { get; private set; }

        [ValidateUri]
        public string WebServiceUrl { get; private set; }
    }
}