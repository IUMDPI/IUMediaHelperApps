using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml.Serialization;
using Packager.Attributes;
using Packager.Extensions;
using Packager.Validators.Attributes;

namespace Packager.Models
{
    public class ProgramSettings : IProgramSettings
    {
        public ProgramSettings(NameValueCollection settings)
        {
            ImportMappedConfigStringSettings(settings);
            ImportMappedConfigListSettings(settings);

            PodAuth = GetAuthorization(settings["PodAuthorizationFile"]);
        }

        [FromConfigSetting("PathToMetaEdit")]
        [Required]
        [ValidateFile]
        public string BWFMetaEditPath { get; private set; }

        [FromConfigSetting("PathToFFMpeg")]
        [Required]
        [ValidateFile]
        public string FFMPEGPath { get; private set; }

        [FromConfigSetting("WhereStaffWorkDirectoryName")]
        [Required]
        [ValidateFolder]
        public string InputDirectory { get; private set; }

        [FromConfigSetting("ProcessingDirectoryName")]
        [Required]
        [ValidateFolder]
        public string ProcessingDirectory { get; private set; }

        [FromConfigSetting("ffmpegAudioProductionArguments")]
        [Required]
        public string FFMPEGAudioProductionArguments { get; private set; }

        //[FromConfigSetting("ffmpegAudioAccessArguments")]
        [Required]
        public string FFMPEGAudioAccessArguments { get; private set; }

        [FromConfigSetting("ProjectCode")]
        [Required]
        public string ProjectCode { get; private set; }

        [FromConfigSetting("DropBoxDirectoryName")]
        [ValidateFolder]
        [Required]
        public string DropBoxDirectoryName { get; private set; }

        [ValidateObject]
        public PodAuth PodAuth { get; private set; }

        //[FromConfigSetting("ErrorDirectoryName")]
        [ValidateFolder]
        public string ErrorDirectoryName { get; private set; }

        //[FromConfigSetting("SuccessDirectoryName")]
        [ValidateFolder]
        public string SuccessDirectoryName { get; private set; }

        [FromConfigSetting("LogDirectoryName")]
        [ValidateFolder]
        [Required]
        public string LogDirectoryName { get; private set; }

        [FromConfigSettingList("IssueNotifyEmailAddresses")]
        public string[] IssueNotifyEmailAddresses { get; private set; }

        [FromConfigSetting("SmtpServer")]
        public string SmtpServer { get; private set; }

        [FromConfigSetting("FromEmailAddress")]
        public string FromEmailAddress { get; private set; }

        //[FromConfigSetting("WebServiceUrl")]
        [ValidateUri]
        public string WebServiceUrl { get; private set; }

        public string DateFormat
        {
            get { return "yyyy-MM-dd HH:mm:ss \"GMT\"zzz"; }
        }

        /*public void Verify()
        {
            if (!Directory.Exists(InputDirectory))
            {
                throw new DirectoryNotFoundException(InputDirectory);
            }

            if (!Directory.Exists(ProcessingDirectory))
            {
                throw new DirectoryNotFoundException(ProcessingDirectory);
            }

            if (!File.Exists(FFMPEGPath))
            {
                throw new FileNotFoundException(FFMPEGPath);
            }

            if (!File.Exists(BWFMetaEditPath))
            {
                throw new FileNotFoundException(BWFMetaEditPath);
            }

            if (!Directory.Exists(DropBoxDirectoryName))
            {
                throw new DirectoryNotFoundException(DropBoxDirectoryName);
            }

            if (!Directory.Exists(ErrorDirectoryName))
            {
                throw new DirectoryNotFoundException(ErrorDirectoryName);
            }

            if (!Directory.Exists(LogDirectoryName))
            {
                throw new DirectoryNotFoundException(LogDirectoryName);
            }

            if (!Directory.Exists(SuccessDirectoryName))
            {
                throw new DirectoryNotFoundException(SuccessDirectoryName);
            }
        }*/

        private void ImportMappedConfigStringSettings(NameValueCollection settings)
        {
            foreach (var property in GetType().GetProperties()
                .Where(p => p.GetCustomAttribute<FromConfigSettingAttribute>() != null))
            {
                var settingName = property.GetCustomAttribute<FromConfigSettingAttribute>().Name;
                property.SetValue(this, settings[settingName]);
            }
        }

        private void ImportMappedConfigListSettings(NameValueCollection settings)
        {
            foreach (var property in GetType().GetProperties()
                .Where(p => p.GetCustomAttribute<FromConfigSettingListAttribute>() != null))
            {
                var settingName = property.GetCustomAttribute<FromConfigSettingListAttribute>().Name;
                var value = settings[settingName].ToDefaultIfEmpty()
                    .Split(new[] {','}, StringSplitOptions.RemoveEmptyEntries).Select(e => e.Trim()).ToArray();
                property.SetValue(this, value);
            }
        }

        

        private static PodAuth GetAuthorization(string path)
        {
            return new PodAuth();

            var serializer = new XmlSerializer(typeof (PodAuth));
            using (var stream = new FileStream(path, FileMode.Open))
            {
                return (PodAuth) serializer.Deserialize(stream);
            }
        }
    }
}