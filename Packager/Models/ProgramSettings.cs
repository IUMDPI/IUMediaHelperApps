using System;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Xml.Serialization;
using Packager.Extensions;

namespace Packager.Models
{
    public class ProgramSettings : IProgramSettings
    {
        public ProgramSettings(NameValueCollection settings)
        {
            InputDirectory = settings["WhereStaffWorkDirectoryName"];
            ProcessingDirectory = settings["ProcessingDirectoryName"];
            FFMPEGPath = settings["PathToFFMpeg"];
            BWFMetaEditPath = settings["PathToMetaEdit"];
            FFMPEGAudioProductionArguments = settings["ffmpegAudioProductionArguments"];
            FFMPEGAudioAccessArguments = settings["ffmpegAudioAccessArguments"];
            ProjectCode = settings["ProjectCode"];
            DropBoxDirectoryName = settings["DropBoxDirectoryName"];
            BaseWebServiceUrlFormat = settings["BaseWebServiceUrlFormat"];
            DigitizingEntity = settings["DigitizingEntity"];
            ErrorDirectoryName = settings["ErrorDirectoryName"];
            SuccessDirectoryName = settings["SuccessDirectoryName"];
            LogDirectoryName = settings["LogDirectoryName"];
            PodAuth = GetAuthorization(settings["PodAuthorizationFile"]);
            SmtpServer = settings["SmtpServer"];
            FromEmailAddress = settings["FromEmailAddress"];
            IssueNotifyEmailAddresses = settings["IssueNotifyEmailAddresses"].ToDefaultIfEmpty()
                .Split(new[] {','}, StringSplitOptions.RemoveEmptyEntries).Select(e => e.Trim()).ToArray();
        }

        public string BWFMetaEditPath { get; private set; }
        public string FFMPEGPath { get; private set; }
        public string InputDirectory { get; private set; }
        public string ProcessingDirectory { get; private set; }
        public string FFMPEGAudioProductionArguments { get; private set; }
        public string FFMPEGAudioAccessArguments { get; private set; }
        public string ProjectCode { get; private set; }
        public string DropBoxDirectoryName { get; private set; }
        public PodAuth PodAuth { get; private set; }
        public string DigitizingEntity { get; private set; }
        public string ErrorDirectoryName { get; private set; }
        public string SuccessDirectoryName { get; private set; }
        public string LogDirectoryName { get; private set; }
        public string[] IssueNotifyEmailAddresses { get; private set; }
        public string SmtpServer { get; private set; }
        public string FromEmailAddress { get; private set; }

        public string MachineName
        {
            get { return Environment.MachineName; }
        }

        public string DateFormat
        {
            get { return "yyyy-MM-dd HH:mm:ss \"GMT\"zzz"; }
        }

        public void Verify()
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
        }

        public string BaseWebServiceUrlFormat { get; private set; }

        private static PodAuth GetAuthorization(string path)
        {
            var serializer = new XmlSerializer(typeof (PodAuth));
            using (var stream = new FileStream(path, FileMode.Open))
            {
                return (PodAuth) serializer.Deserialize(stream);
            }
        }
    }
}