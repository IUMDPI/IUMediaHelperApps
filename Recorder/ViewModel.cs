using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Windows.Input;
using System.Windows.Media;
using JetBrains.Annotations;
using Recorder.Models;

namespace Recorder
{
    public class ViewModel : INotifyPropertyChanged
    {

        private string WorkingFolderName => Path.Combine(_settings.WorkingFolder, $"{Barcode}_{Part}");

        private Process _process;

        private readonly IProgramSettings _settings;
        private string _barcode;
        private int _part;
        private ICommand _pauseButtonCommand;

        private ICommand _recordButtonCommand;
        private bool _recording;
        private bool _paused;

        public ViewModel(IProgramSettings settings)
        {
            _settings = settings;
            Part = 1;

        }

        public bool LockInputs => !Recording;

        public ICommand RecordButtonCommand
        {
            get { return _recordButtonCommand 
                    ?? (_recordButtonCommand = new RelayCommand(param => DoRecord(), param => CanRecord())); }
        }

        public ICommand PauseButtonCommand
        {
            get
            {
                return _pauseButtonCommand
                   ?? (_pauseButtonCommand = new RelayCommand(param => DoPause(), param => CanPause()));
            }

        }

        private bool CanRecord()
        {
            if (string.IsNullOrEmpty(Barcode))
            {
                return false;
            }

            return !Paused;
        }

        private bool CanPause()
        {
            return Recording;
        }

        private void DoRecord()
        {
            if (Recording)
            {
                _process.StandardInput.WriteLine('q');
                Recording = false;
            }
            else
            {
                // make folder in temp directory
                if (!Directory.Exists(WorkingFolderName))
                {
                    Directory.CreateDirectory(WorkingFolderName);
                }

                var info = new ProcessStartInfo(_settings.PathToFFMPEG);
                info.Arguments = $"{_settings.FFMPEGArguments} \"{GetNewPart()}\"";
                info.RedirectStandardInput = true;
                info.UseShellExecute = false;
                _process = Process.Start(info);
                Recording = true;
            }
        }

        private string GetNewPart()
        {
            var count = 0;
            var value = Path.Combine(WorkingFolderName, $"part_{count}.mkv");
            while (File.Exists(value))
            {
                count++;
                value = Path.Combine(WorkingFolderName, $"part_{count}.mkv");
                Thread.Sleep(5);
            }

            return value;
        }

        private void DoPause()
        {
            Paused = !Paused;
        }

        public Brush RecordButtonForeground => Recording ? new SolidColorBrush(Colors.Black) : new SolidColorBrush(Colors.Red);
        public Brush RecordButtonBackground => Recording ? new SolidColorBrush(Colors.Red) : new SolidColorBrush(Colors.LightGray);

        public string RecordingButtonLabel => Recording ? "<" : "=";

        public bool Recording
        {
            get { return _recording; }
            set
            {
                _recording = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(RecordingButtonLabel));
                OnPropertyChanged(nameof(RecordButtonBackground));
                OnPropertyChanged(nameof(RecordButtonForeground));
                OnPropertyChanged(nameof(LockInputs));
            }
        }

        public bool Paused
        {
            get { return _paused; }
            set { _paused = value; OnPropertyChanged(); }
        }

        public string Barcode
        {
            get { return _barcode; }
            set
            {
                _barcode = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(RecordButtonCommand));
            }
        }

        public int Part
        {
            get { return _part; }
            set
            {
                _part = value;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}