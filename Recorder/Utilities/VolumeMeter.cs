using System;
using System.Diagnostics;
using System.Text.RegularExpressions;
using NAudio.Wave;
using Recorder.Models;

namespace Recorder.Utilities
{
    public class VolumeMeter : IDisposable
    {
        private readonly WaveIn _waveIn;

        public VolumeMeter(IProgramSettings settings)
        {
            _waveIn = new WaveIn {DeviceNumber = FindDevice(GetDeviceName(settings.FFMPEGArguments)) };

            _waveIn.DataAvailable += DataAvailableHandler;
            _waveIn.RecordingStopped += RecordingStoppedHandler;

            SampleAggregator = new SampleAggregator {NotificationCount = new WaveFormat(96000,1).SampleRate / 40};
        }

        private void RecordingStoppedHandler(object sender, StoppedEventArgs e)
        {
            SampleAggregator.ResetValues();
        }

        public SampleAggregator SampleAggregator { get; }

        public void Dispose()
        {
            _waveIn?.Dispose();
        }

        private static string GetDeviceName(string arguments)
        {
            var match = Regex.Match(arguments, "(:audio=)\"(.*)\"");
            if (match.Success == false)
            {
                return "";
            }

            if (match.Groups.Count < 3)
            {
                return "";
            }

            return match.Groups[2].Value;
        }

        public void StartMonitoring()
        {
            if (_waveIn.DeviceNumber < 0)
            {
                return;
            }
            
            SampleAggregator.ResetValues();
            _waveIn.StartRecording();
        }

        public void StopMonitoring()
        {
            _waveIn.StopRecording();
        }

        private void DataAvailableHandler(object sender, WaveInEventArgs e)
        {
            var buffer = e.Buffer;

            for (var index = 0; index < e.BytesRecorded; index += 2)
            {
                var sample = (short) ((buffer[index + 1] << 8) |
                                      buffer[index + 0]);
                var sample32 = sample/32768f;
                SampleAggregator.Add(sample32);
            }
        }

        private static int FindDevice(string deviceName)
        {
            var deviceCount = WaveIn.DeviceCount;
            for (var index = 0; index < deviceCount; index++)
            {
                var deviceInfo = WaveIn.GetCapabilities(index);
                if (deviceInfo.ProductName.Equals(deviceName))
                {
                    return index;
                }
            }

            return -1;
        }
    }

    public class SampleAggregator
    {
        private int _count;
        private float _maxValue;
        private float _minValue;
        public int NotificationCount { get; set; }
        public event EventHandler<MaxSampleEventArgs> MaximumCalculated;
        
        private void Reset()
        {
            _count = 0;
            _maxValue = _minValue = 0;
        }

        public void ResetValues()
        {
            Reset();
            MaximumCalculated?.Invoke(this, new MaxSampleEventArgs(_minValue, _maxValue));
        }

        public void Add(float value)
        {
            _maxValue = Math.Max(_maxValue, value);
            _minValue = Math.Min(_minValue, value);
            _count++;
            if (_count < NotificationCount || NotificationCount <= 0) return;
            
            MaximumCalculated?.Invoke(this, new MaxSampleEventArgs(_minValue, _maxValue));
            Reset();
        }
    }

    public class MaxSampleEventArgs : EventArgs
    {
        [DebuggerStepThrough]
        public MaxSampleEventArgs(float minValue, float maxValue)
        {
            MaxSample = maxValue;
            MinSample = minValue;
        }

        public float MaxSample { get; private set; }
        public float MinSample { get; private set; }
    }
}