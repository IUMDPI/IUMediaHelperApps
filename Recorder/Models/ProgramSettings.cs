using System;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Security.RightsManagement;
using Recorder.Exceptions;

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

        string AudioDeviceToMonitor { get; }

        void Verify();
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
            AudioDeviceToMonitor = settings["AudioDeviceToMonitor"];
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
        public string AudioDeviceToMonitor { get; }

        public void Verify()
        {
            if (string.IsNullOrWhiteSpace(ProjectCode))
            {
                throw new ConfigurationException("ProjectCode is not set in app.config");
            }

            if (string.IsNullOrWhiteSpace(PathToFFMPEG) || !File.Exists(PathToFFMPEG))
            {
                throw new ConfigurationException("PathToFFMPEG in app.config is not set or invalid");
            }

            if (string.IsNullOrWhiteSpace(PathToFFMPEG) || !File.Exists(PathToFFProbe))
            {
                throw new ConfigurationException("PathToFFProbe in app.config is not set or invalid");
            }

            if (string.IsNullOrWhiteSpace(FFMPEGArguments))
            {
                throw new ConfigurationException("FFMPEGArguments are not set in app.config");
            }

            if (string.IsNullOrWhiteSpace(OutputFolder) || !Directory.Exists(OutputFolder))
            {
                throw new ConfigurationException("OutputFolder in app.config is not set or invalid");
            }

            if (string.IsNullOrWhiteSpace(WorkingFolder) || !Directory.Exists(WorkingFolder))
            {
                throw new ConfigurationException("OutputFolder in app.config is not set or invalid");
            }
        }
    }
}