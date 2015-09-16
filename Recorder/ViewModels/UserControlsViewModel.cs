using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using JetBrains.Annotations;
using Recorder.Models;
using Recorder.Utilities;

namespace Recorder.ViewModels
{
    public class UserControlsViewModel : INotifyPropertyChanged
    {
        private readonly List<AbstractPanelViewModel> _panels;

        private readonly IProgramSettings _settings;

        public UserControlsViewModel(IProgramSettings settings, RecordingEngine recorder)
        {
            _settings = settings;

            _panels = new List<AbstractPanelViewModel>
            {
                new BarcodePanelViewModel(this, recorder, _settings.ProjectCode) {Visibility = Visibility.Visible},
                new RecordPanelViewModel(this, recorder) {Visibility = Visibility.Collapsed},
                new FinishPanelViewModel(this, recorder) {Visibility = Visibility.Collapsed}
            };
            ActionPanelViewModel = new ActionPanelViewModel(this);
        }

        public string Title => $"{_settings.ProjectCode} Recorder";
        
        public ActionPanelViewModel ActionPanelViewModel { get; private set; }
        public BarcodePanelViewModel BarcodePanelViewModel => GetPanel<BarcodePanelViewModel>();
        public RecordPanelViewModel RecordPanelViewModel => GetPanel<RecordPanelViewModel>();
        public FinishPanelViewModel FinishPanelViewModel => GetPanel<FinishPanelViewModel>();

        public AbstractPanelViewModel ActivePanelModel => _panels.Single(p => p.Visibility == Visibility.Visible);
        //public bool LockInputs => !Recording;
/*
        public ICommand RecordButtonCommand
        {
            get
            {
                return _recordButtonCommand
                       ?? (_recordButtonCommand = new RelayCommand(param => DoRecord(), param => CanRecord()));
            }
        }

        public ICommand PauseButtonCommand
        {
            get
            {
                return _pauseButtonCommand
                       ?? (_pauseButtonCommand = new RelayCommand(param => DoPause(), param => CanPause()));
            }
        }*/

        /*public Brush RecordButtonForeground => Recording ? new SolidColorBrush(Colors.Black) : new SolidColorBrush(Colors.Red);
        public Brush RecordButtonBackground => Recording ? new SolidColorBrush(Colors.Red) : new SolidColorBrush(Colors.LightGray);

        public string RecordingButtonLabel => Recording ? "<" : "=";
*/
        /*public bool Recording
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
            set
            {
                _paused = value;
                OnPropertyChanged();
            }
        }*/


        public event PropertyChangedEventHandler PropertyChanged;

        private T GetPanel<T>() where T : AbstractPanelViewModel
        {
            return _panels.Single(p => p.GetType() == typeof (T)) as T;
        }

/*

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
*/
/*
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
        }*/

        /*private void DoPause()
        {
            Paused = !Paused;
        }*/

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public void ShowPanel<T>() where T : AbstractPanelViewModel
        {
            foreach (var panel in _panels)
            {
                panel.Visibility = panel.GetType() == typeof (T) ? Visibility.Visible : Visibility.Collapsed;
            }
            OnPropertyChanged(nameof(ActivePanelModel));
        }
    }
}