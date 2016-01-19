using System.Collections.Specialized;
using Packager.Factories;
using Packager.Validators.Attributes;

namespace Packager.Models.SettingsModels
{
    public class ProgramSettings : IProgramSettings
    {
   
        [ValidateFile]
        public string PodAuthFilePath { get; set; }

        [ValidateFile]
        public string BwfMetaEditPath { get; set; }

        [ValidateFile]
        public string FFMPEGPath { get; set; }

        [ValidateFile]
        public string FFProbePath { get; set; }

        [ValidateFolder]
        public string InputDirectory { get; set; }

        [ValidateFolder]
        public string ProcessingDirectory { get; set; }

        [Required]
        public string FFMPEGAudioProductionArguments { get; set; }

        [Required]
        public string FFMPEGAudioAccessArguments { get; set; }

        [Required]
        public string FFMPEGVideoMezzanineArguments { get; set; }

        [Required]
        public string FFMPEGVideoAccessArguments { get; set; }

        [Required]
        public string FFProbeVideoQualityControlArguments { get; set; }

        [Required]
        public string ProjectCode { get; set; }

        [ValidateFolder]
        public string DropBoxDirectoryName { get; set; }

        [ValidateFolder]
        public string ErrorDirectoryName { get; set; }

        [ValidateFolder]
        public string SuccessDirectoryName { get; set; }

        [ValidateFolder]
        public string LogDirectoryName { get; set; }

        public string[] IssueNotifyEmailAddresses { get; set; }

        [Required]
        public string UnitPrefix { get; set; }

        public string SmtpServer { get; set; }

        public string FromEmailAddress { get; set; }

        public int DeleteSuccessfulObjectsAfterDays { get; set; }

        [ValidateUri]
        public string WebServiceUrl { get; set; }
    }
}