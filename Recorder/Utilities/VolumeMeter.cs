using System;
using System.Diagnostics;
using NAudio.Wave;
using Recorder.Models;
using Recorder.ViewModels;

namespace Recorder.Utilities
{
    public class VolumeMeter : IDisposable, ICanLogConfiguration
    {
        private readonly WaveIn _waveIn;
        private readonly string _deviceName;

        public VolumeMeter(IProgramSettings settings)
        {
            _deviceName = settings.AudioDeviceToMonitor;
            _waveIn = new WaveIn {DeviceNumber = FindDevice(_deviceName) };
          
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
        
        public void LogConfiguration(OutputWindowViewModel outputModel)
        {
            if (_waveIn.DeviceNumber >= 0)
            {
                outputModel.WriteLine("Audio meter: initialized");
                return;
            }

            outputModel.WriteLine($"Audio meter: could not initialize audio meter for device {_deviceName}.");
            outputModel.WriteLine("Audio meter: eligible devices are: ");
            var deviceCount = WaveIn.DeviceCount;
            for (var index = 0; index < deviceCount; index++)
            {
                var deviceInfo = WaveIn.GetCapabilities(index);
                outputModel.WriteLine($"   {deviceInfo.ProductName}");
            }
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