using System.Collections.Specialized;

namespace Recorder.Models
{
    public interface IProgramSettings
    {
        string ProjectCode { get; }
        string PathToFFMPEG { get; }
        string OutputFolder { get; }
        string FFMPEGArguments { get; }

        string WorkingFolder { get; }
    }

    public class ProgramSettings : IProgramSettings
    {
        public ProgramSettings(NameValueCollection settings)
        {
            ProjectCode = settings["ProjectCode"].ToUpperInvariant();
            PathToFFMPEG = settings["PathToFFMPEG"];
            OutputFolder = settings["OutputDirectoryName"];
            FFMPEGArguments = settings["FFMPEGArguments"];
            WorkingFolder = settings["WorkingDirectoryName"];

        }

        public string ProjectCode { get; }
        public string PathToFFMPEG { get; }
        public string OutputFolder { get; }
        public string FFMPEGArguments { get; }
        public string WorkingFolder { get; }
    }
}