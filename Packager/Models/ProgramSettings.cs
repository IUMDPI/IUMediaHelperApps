using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml.Serialization;
using Packager.Attributes;
using Packager.Attributes.Packager.Attributes;
using Packager.Exceptions;
using Packager.Extensions;
using Packager.Utilities;
using Packager.Validators.Attributes;

namespace Packager.Models
{
    public class ProgramSettings : IProgramSettings
    {
        public ProgramSettings(NameValueCollection settings)
        {
            ImportMappedConfigStringSettings(settings);
            ImportMappedConfigIntegerSettings(settings);
            ImportMappedConfigListSettings(settings);
            ImportMappedConfigEnumListSettings<BextFields>(settings);

            PodAuth = GetAuthorization(settings["PodAuthorizationFile"]);
        }

        [FromConfigSetting("PathToMetaEdit")]
        [ValidateFile]
        public string BwfMetaEditPath { get; private set; }

        [FromConfigSetting("PathToFFMpeg")]
        [ValidateFile]
        public string FFMPEGPath { get; private set; }

        [FromConfigSetting("WhereStaffWorkDirectoryName")]
        [ValidateFolder]
        public string InputDirectory { get; private set; }

        [FromConfigSetting("ProcessingDirectoryName")]
        [ValidateFolder]
        public string ProcessingDirectory { get; private set; }

        [FromConfigSetting("ffmpegAudioProductionArguments")]
        [Required]
        public string FFMPEGAudioProductionArguments { get; private set; }

        [FromConfigSetting("ffmpegAudioAccessArguments")]
        [Required]
        public string FFMPEGAudioAccessArguments { get; private set; }

        [FromConfigSetting("ProjectCode")]
        [Required]
        public string ProjectCode { get; private set; }

        [FromConfigSetting("DropBoxDirectoryName")]
        [ValidateFolder]
        public string DropBoxDirectoryName { get; private set; }

        [ValidateObject]
        public PodAuth PodAuth { get; private set; }

        [FromConfigSetting("ErrorDirectoryName")]
        [ValidateFolder]
        public string ErrorDirectoryName { get; private set; }

        [FromConfigSetting("SuccessDirectoryName")]
        [ValidateFolder]
        public string SuccessDirectoryName { get; private set; }

        [FromConfigSetting("LogDirectoryName")]
        [ValidateFolder]
        [Required]
        public string LogDirectoryName { get; private set; }

        [FromConfigSettingList("IssueNotifyEmailAddresses")]
        public string[] IssueNotifyEmailAddresses { get; private set; }

        [FromConfigSettingEnumList("SuppressAudioMetadataFields")]
        public BextFields[] SuppressAudioMetadataFields { get; private set; }

        [FromConfigSetting("SmtpServer")]
        public string SmtpServer { get; private set; }

        [FromConfigSetting("FromEmailAddress")]
        public string FromEmailAddress { get; private set; }

        [FromIntegerConfigSetting("DeleteProcessedAfterInDays")]
        public int DeleteSuccessfulObjectsAfterDays { get; private set; }

        [FromConfigSetting("WebServiceUrl")]
        [ValidateUri]
        public string WebServiceUrl { get; private set; }

        public string DateFormat => "yyyy-MM-dd HH:mm:ss \"GMT\"zzz";

        private void ImportMappedConfigStringSettings(NameValueCollection settings)
        {
            foreach (var property in GetType().GetProperties()
                .Where(p => p.GetCustomAttribute<FromConfigSettingAttribute>() != null))
            {
                var settingName = property.GetCustomAttribute<FromConfigSettingAttribute>().Name;
                property.SetValue(this, settings[settingName]);
            }
        }

        private void ImportMappedConfigIntegerSettings(NameValueCollection settings)
        {
            foreach (var property in GetType().GetProperties()
                .Where(p => p.GetCustomAttribute<FromIntegerConfigSettingAttribute>() != null))
            {
                var settingName = property.GetCustomAttribute<FromIntegerConfigSettingAttribute>().Name;

                int value;

                if (int.TryParse(settings[settingName], out value))
                {
                    property.SetValue(this, value);
                }
            }
        }

        private void ImportMappedConfigListSettings(NameValueCollection settings)
        {
            foreach (var property in GetType().GetProperties()
                .Where(p => p.GetCustomAttribute<FromConfigSettingListAttribute>() != null))
            {
                var settingName = property.GetCustomAttribute<FromConfigSettingListAttribute>().Name;
                var value = settings[settingName].ToDefaultIfEmpty()
                    .Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(e => e.Trim()).ToArray();
                property.SetValue(this, value);
            }
        }

        private void ImportMappedConfigEnumListSettings<T>(NameValueCollection settings) where T : struct
        {
            foreach (var property in GetType().GetProperties()
                .Where(p => p.PropertyType.GetElementType() == typeof(T))
                .Where(p => p.GetCustomAttribute<FromConfigSettingEnumListAttribute>() != null))
            {
                var settingName = property.GetCustomAttribute<FromConfigSettingEnumListAttribute>().Name;
                var values = settings[settingName].ToDefaultIfEmpty()
                    .Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(e => e.Trim()).ToArray();

                // need to make th
                var toSetValues = new List<T>();

                foreach (var value in values)
                {
                    T enumValue;
                    if (Enum.TryParse(value, true, out enumValue) == false)
                    {
                        continue;
                    }

                    toSetValues.Add(enumValue);
                }
                property.SetValue(this, toSetValues.ToArray());
            }
        }

        private static PodAuth GetAuthorization(string path)
        {
            var serializer = new XmlSerializer(typeof(PodAuth));
            using (var stream = new FileStream(path, FileMode.Open))
            {
                return (PodAuth)serializer.Deserialize(stream);
            }
        }
    }
}