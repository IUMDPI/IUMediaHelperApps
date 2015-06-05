using System.Collections.Specialized;
using System.IO;
using System.Xml.Serialization;

namespace Packager.Models
{
    public interface IProgramSettings
    {
        // ReSharper disable once InconsistentNaming
        string BWFMetaEditPath { get; }
        // ReSharper disable once InconsistentNaming
        string FFMPEGPath { get; }
        string InputDirectory { get; }
        string ProcessingDirectory { get; }
        // ReSharper disable once InconsistentNaming
        string FFMPEGAudioProductionArguments { get; }
        // ReSharper disable once InconsistentNaming
        string FFMPEGAudioAccessArguments { get; }
        string ProjectCode { get; }
        string DropBoxDirectoryName { get; }
        string DateFormat { get; }
        PodAuth PodAuth { get; }
        string BaseWebServiceUrlFormat { get; }
        void Verify();
    }

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

            PodAuth = GetAuthorization(settings["PodAuthorizationFile"]);
        }

        public string BWFMetaEditPath { get; private set; }
        public string FFMPEGPath { get; private set; }
        public string InputDirectory { get; private set; }
        public string ProcessingDirectory { get; private set; }
        public string FFMPEGAudioProductionArguments { get; private set; }
        public string FFMPEGAudioAccessArguments { get; private set; }
        public string ProjectCode { get; private set; }
        public string DropBoxDirectoryName { get; private set; }

        public string DateFormat
        {
            get { return "yyyy-MM-dd HH:mm:ss \"GMT\"zzz"; }
        }

        public PodAuth PodAuth { get; private set; }

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