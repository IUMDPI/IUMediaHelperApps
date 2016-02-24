using System;
using System.Collections.Specialized;
using System.Linq;
using Packager.Extensions;
using Packager.Models.SettingsModels;

namespace Packager.Factories
{
    public class SettingsFactory
    {
        public static IProgramSettings Import(NameValueCollection settings)
        {
            return new ProgramSettings
            {
                BwfMetaEditPath = GetStringValue(settings, "PathToMetaEdit"),
                FFMPEGPath = GetStringValue(settings, "PathToFFMpeg"),
                FFProbePath = GetStringValue(settings, "PathToFFProbe"),
                InputDirectory = GetStringValue(settings, "WhereStaffWorkDirectoryName"),
                ProcessingDirectory = GetStringValue(settings, "ProcessingDirectoryName"),
                FFMPEGAudioProductionArguments = GetStringValue(settings, "ffmpegAudioProductionArguments"),
                FFMPEGAudioAccessArguments = GetStringValue(settings, "ffmpegAudioAccessArguments"),
                FFMPEGVideoMezzanineArguments = GetStringValue(settings, "ffmpegVideoMezzanineArguments"),
                FFMPEGVideoAccessArguments = GetStringValue(settings, "ffmpegVideoAccessArguments"),
                FFProbeVideoQualityControlArguments = GetStringValue(settings,
                    "ffprobeVideoQualityControlArguments"),
                ProjectCode = GetStringValue(settings, "ProjectCode"),
                DropBoxDirectoryName = GetStringValue(settings, "DropBoxDirectoryName"),
                ErrorDirectoryName = GetStringValue(settings, "ErrorDirectoryName"),
                SuccessDirectoryName = GetStringValue(settings, "SuccessDirectoryName"),
                LogDirectoryName = GetStringValue(settings, "LogDirectoryName"),

                UnitPrefix = GetStringValue(settings, "UnitPrefix"),
                SmtpServer = GetStringValue(settings, "SmtpServer"),
                WebServiceUrl = GetStringValue(settings, "WebServiceUrl"),
                PodAuthFilePath = GetStringValue(settings, "PodAuthorizationFile"),
                FromEmailAddress = GetStringValue(settings, "FromEmailAddress"),

                IssueNotifyEmailAddresses = GetStringValues(settings, "IssueNotifyEmailAddresses"),
                DeleteSuccessfulObjectsAfterDays = GetIntValue(settings, "DeleteProcessedAfterInDays", 0),

                DigitizingEntity = GetStringValue(settings, "DigitizingEntity")
            };
        }

        public static string GetStringValue(NameValueCollection settings, string name)
        {
            return settings[name].ToDefaultIfEmpty();
        }

        public static int GetIntValue(NameValueCollection settings, string name, int defaultValue)
        {
            int result;
            return int.TryParse(GetStringValue(settings, name), out result)
                ? result
                : defaultValue;
        }

        public static string[] GetStringValues(NameValueCollection settings, string name)
        {
            return GetStringValue(settings, name)
                .Split(new[] {','}, StringSplitOptions.RemoveEmptyEntries)
                .Select(e => e.Trim()) // trim initial and trailing spaces
                .Where(e => e.IsSet()) // remove entries that are empty (just in case)
                .ToArray();
        }
    }
}