using System;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using Recorder.Exceptions;
using Recorder.Utilities;
using Recorder.ViewModels;

namespace Recorder.Models
{
    public interface IProgramSettings : ICanLogConfiguration
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
            BarcodeScannerIdentifiers = ToArray(settings["BarcodeScannerIdentifiers"]);
            AudioDeviceToMonitor = settings["AudioDeviceToMonitor"];
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

        public void LogConfiguration(OutputWindowViewModel outputModel)
        {
            outputModel.WriteLine($"Project code: {ProjectCode}");
            outputModel.WriteLine($"FFMPEG Path: {PathToFFMPEG}");
            outputModel.WriteLine($"FFProbe Path: {PathToFFProbe}");
            outputModel.WriteLine($"FFMPEG Arguments: {FFMPEGArguments}");
            outputModel.WriteLine($"Working folder: {WorkingFolder}");
            outputModel.WriteLine($"Output folder: {OutputFolder}");
            outputModel.WriteLine($"Barcode scanner identifiers: {string.Join(", ", BarcodeScannerIdentifiers)}");
            outputModel.WriteLine($"Audio device to monitor: {AudioDeviceToMonitor}");
        }


        private static string[] ToArray(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return new string[0];
            }

            return value.Split(new[] {','}, StringSplitOptions.RemoveEmptyEntries)
                .Select(s => s.Trim()).ToArray();
        }
    }
}