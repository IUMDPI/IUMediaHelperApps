using System;
using System.Collections.Specialized;
using System.Linq;

namespace Recorder.Models
{
    public interface IProgramSettings
    {
        string ProjectCode { get; }
        string PathToFFMPEG { get; }
        string OutputFolder { get; }

        string PathToFFProbe { get; }
        string FFMPEGArguments { get; }

        string WorkingFolder { get; }
        
        string[] BarcodeScannerIdentifiers { get; }
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
            PathToFFProbe = settings["PathToFFProbe"];
            BarcodeScannerIdentifiers =ToArray(settings["BarcodeScannerIdentifiers"]);
        }

        private static string[] ToArray(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return new string[0];
            }

            return value.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                    .Select(s => s.Trim()).ToArray();
        }
        

        public string ProjectCode { get; }
        public string PathToFFMPEG { get; }
        public string OutputFolder { get; }
        public string PathToFFProbe { get; }
        public string FFMPEGArguments { get; }
        public string WorkingFolder { get; }
        public string[] BarcodeScannerIdentifiers { get; }
    }
}