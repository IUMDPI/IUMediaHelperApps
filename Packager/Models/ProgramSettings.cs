﻿using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Reflection;
using Packager.Attributes;
using Packager.Extensions;
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

        [FromStringConfigSetting("PathToFFProbe")]
        [ValidateFile]
        public string FFProbePath { get; private set; }

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

        [FromStringConfigSetting("ffmpegVideoMezzanineArguments")]
        [Required]
        public string FFMPEGVideoMezzanineArguments { get; private set; }

        [FromStringConfigSetting("ffmpegVideoAccessArguments")]
        [Required]
        public string FFMPEGVideoAccessArguments { get; private set; }

        [FromStringConfigSetting("ffprobeVideoQualityControlArguments")]
        [Required]
        public string FFProbeVideoQualityControlArguments { get; private set; }

        [FromStringConfigSetting("ffmpegLogLevel")]
        public string FFMPEGLogLevel { get; private set; }

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

        public List<string> Issues { get; }

        [FromStringConfigSetting("UnitPrefix")]
        public string UnitPrefix { get; private set; }

        [FromStringConfigSetting("SmtpServer")]
        public string SmtpServer { get; private set; }

        [FromStringConfigSetting("FromEmailAddress")]
        public string FromEmailAddress { get; private set; }

        [FromIntegerConfigSetting("DeleteProcessedAfterInDays")]
        public int DeleteSuccessfulObjectsAfterDays { get; private set; }

        [FromStringConfigSetting("WebServiceUrl")]
        [ValidateUri]
        public string WebServiceUrl { get; private set; }

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