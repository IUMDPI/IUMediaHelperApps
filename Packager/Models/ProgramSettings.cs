using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Reflection;
using Packager.Attributes;
using Packager.Attributes.Packager.Attributes;
using Packager.Extensions;
using Packager.Utilities;
using Packager.Validators.Attributes;

namespace Packager.Models
{
    public class ProgramSettings : IProgramSettings
    {
        public ProgramSettings(NameValueCollection settings)
        {
            Issues = new List<string>();
            ImportMappedConfigSettings(settings);
        }

        [FromStringConfigSetting("PathToMetaEdit")]
        [ValidateFile]
        public string BwfMetaEditPath { get; private set; }

        [FromStringConfigSetting("PathToFFMpeg")]
        [ValidateFile]
        public string FFMPEGPath { get; private set; }

        [FromStringConfigSetting("WhereStaffWorkDirectoryName")]
        [ValidateFolder]
        public string InputDirectory { get; private set; }

        [FromStringConfigSetting("ProcessingDirectoryName")]
        [ValidateFolder]
        public string ProcessingDirectory { get; private set; }

        [FromStringConfigSetting("ffmpegAudioProductionArguments")]
        [Required]
        public string FFMPEGAudioProductionArguments { get; private set; }

        [FromStringConfigSetting("ffmpegAudioAccessArguments")]
        [Required]
        public string FFMPEGAudioAccessArguments { get; private set; }

        [FromStringConfigSetting("ProjectCode")]
        [Required]
        public string ProjectCode { get; private set; }

        [FromStringConfigSetting("DropBoxDirectoryName")]
        [ValidateFolder]
        public string DropBoxDirectoryName { get; private set; }

        [ValidateObject]
        [FromPodAuthPathSetting("PodAuthorizationFile")]
        public PodAuth PodAuth { get; private set; }

        [FromStringConfigSetting("ErrorDirectoryName")]
        [ValidateFolder]
        public string ErrorDirectoryName { get; private set; }

        [FromStringConfigSetting("SuccessDirectoryName")]
        [ValidateFolder]
        public string SuccessDirectoryName { get; private set; }

        [FromStringConfigSetting("LogDirectoryName")]
        [ValidateFolder]
        [Required]
        public string LogDirectoryName { get; private set; }

        [FromListConfigSettingAttribute("IssueNotifyEmailAddresses")]
        public string[] IssueNotifyEmailAddresses { get; private set; }

        [FromBextFieldListConfig("SuppressAudioMetadataFields")]
        public BextFields[] SuppressAudioMetadataFields { get; private set; }

        [FromBoolConfigSetting("UseAppendFlagForAudioMetadata")]
        public bool UseAppendFlagForAudioMetadata { get; private set; }

        public List<string> Issues { get; }

        [FromStringConfigSetting("SmtpServer")]
        public string SmtpServer { get; private set; }

        [FromStringConfigSetting("FromEmailAddress")]
        public string FromEmailAddress { get; private set; }

        [FromIntegerConfigSetting("DeleteProcessedAfterInDays")]
        public int DeleteSuccessfulObjectsAfterDays { get; private set; }

        [FromStringConfigSetting("WebServiceUrl")]
        [ValidateUri]
        public string WebServiceUrl { get; private set; }

        public string DateFormat => "yyyy-MM-dd HH:mm:ss \"GMT\"zzz";


        private void ImportMappedConfigSettings(NameValueCollection settings)
        {
            foreach (var property in GetType().GetProperties()
                .Where(p => p.GetCustomAttribute<AbstractFromConfigSettingAttribute>() != null))
            {
                var attribute = property.GetCustomAttribute<AbstractFromConfigSettingAttribute>();

                var value = ConvertAndAddIssues(attribute, settings[attribute.Name].ToDefaultIfEmpty());
                if (value == null)
                {
                    continue;
                }

                property.SetValue(this, value);
            }
        }

        private object ConvertAndAddIssues(AbstractFromConfigSettingAttribute attribute, string value)
        {
            var result = attribute.Convert(value);
            foreach (var issue in attribute.Issues)
            {
                Issues.Add($"Problem importing setting {attribute.Name}: {issue}");
            }

            return result;
        }
    }
}