using System.Collections.Specialized;
using System.IO;

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
        string FFMPEGAudioMezzanineArguments { get; }
        // ReSharper disable once InconsistentNaming
        string FFMPEGAudioAccessArguments { get; }

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
            FFMPEGAudioMezzanineArguments = settings["ffmpegAudioMezzanineArguments"];
            FFMPEGAudioAccessArguments = settings["ffmpegAudioAccessArguments"];
        }

        public string BWFMetaEditPath { get; private set; }
        public string FFMPEGPath { get; private set; }
        public string InputDirectory { get; private set; }
        public string ProcessingDirectory { get; private set; }
        public string FFMPEGAudioMezzanineArguments { get; private set; }
        public string FFMPEGAudioAccessArguments { get; private set; }

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
        }
    }
}