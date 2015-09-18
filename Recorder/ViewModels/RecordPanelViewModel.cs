using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using Recorder.Models;
using Recorder.Utilities;

namespace Recorder.ViewModels
{
    public class RecordPanelViewModel : AbstractPanelViewModel
    {
        private ICommand _backCommand;
        private ICommand _clearCommand;
        private ICommand _nextCommand;
        private TimeSpan _partTimestamp;
        private ICommand _recordCommand;
        private TimeSpan _timestamp;

        public RecordPanelViewModel(UserControlsViewModel parent, ObjectModel objectModel, RecordingEngine recorder) : base(parent, objectModel)
        {
            Recorder = recorder;
            PartTimestamp = new TimeSpan();
            Recorder.TimestampUpdated += TimestampUpdatedHandler;
            Recorder.PropertyChanged += RecorderPropertyChangedHandler;
        }

        private void RecorderPropertyChangedHandler(object sender, PropertyChangedEventArgs e)
        {
            if (!Dispatcher.CurrentDispatcher.CheckAccess())
            {
                Dispatcher.CurrentDispatcher.Invoke(() => RecorderPropertyChangedHandler(sender, e));
                return;
            }

            OnPropertyChanged(nameof(RecordCommand));
        }

        private void RecordingEndedHandler(object sender, EventArgs e)
        {
            if (!Dispatcher.CurrentDispatcher.CheckAccess())
            {
                Dispatcher.CurrentDispatcher.Invoke(() => RecordingEndedHandler(sender, e));
                return;
            }

            OnPropertyChanged(nameof(RecordCommand));
        }

        private void TimestampUpdatedHandler(object sender, TimeSpan e)
        {
            if (!Dispatcher.CurrentDispatcher.CheckAccess())
            {
                Dispatcher.CurrentDispatcher.Invoke(() => TimestampUpdatedHandler(sender,e));
                return;
            }

            var value = Recorder.CumulativeTimeSpan.Add(e);
            CumulativeTimestamp = value;
            PartTimestamp = e;
        }

        private RecordingEngine Recorder { get; }

        public bool ShowClear => ObjectModel.PartsPresent() && !Recorder.Recording;

        public bool ShowPartTimestamp => ObjectModel.MultiplePartsPresent();

        public TimeSpan PartTimestamp
        {
            get { return _partTimestamp; }
            set
            {
                Debug.WriteLine(value);
                _partTimestamp = value;
                OnPropertyChanged();
            }
        }

        public SolidColorBrush RecordButtonForeground => Recorder.Recording
            ? new SolidColorBrush(Colors.Black)
            : new SolidColorBrush(Colors.DarkRed);

        public SolidColorBrush RecordButtonBackground => Recorder.Recording
            ? new SolidColorBrush(Colors.Gray)
            : new SolidColorBrush(Colors.Green);

        public string RecordButtonLabel => Recorder.Recording ? "<" : "=";

        public string RecordCaption
        {
            get
            {
                if (Recorder.Recording)
                {
                    return "Pause / stop recording";
                }

                return ObjectModel.PartsPresent()
                    ? "Resume recording"
                    : "Start recording";
            }
        }

        public TimeSpan CumulativeTimestamp
        {
            get { return _timestamp; }
            set
            {
                _timestamp = value;
                OnPropertyChanged();
            }
        }

        public override string BackButtonText => "Create new object";
        public override string NextButtonText => "Generate single file";

        public override string Instructions => "Click record to start recording.";

        public override ICommand NextCommand
        {
            get
            {
                return _nextCommand
                       ?? (_nextCommand = new RelayCommand(param => Parent.ShowPanel<FinishPanelViewModel>()));
            }
        }

        public override ICommand BackCommand
        {
            get
            {
                return _backCommand
                       ?? (_backCommand = new RelayCommand(param => Parent.ShowPanel<BarcodePanelViewModel>()));
            }
        }

        public ICommand RecordCommand
        {
            get
            {
                return _recordCommand ?? (_recordCommand =
                    new RelayCommand(param => DoRecordAction(), param => Recorder.OkToRecord().IsValid));
            }
        }

        public ICommand ClearCommand
        {
            get
            {
                return _clearCommand ?? (_clearCommand =
                    new RelayCommand(param => DoClearAction(), param => ShowClear));
            }
        }
        
        public override void Initialize()
        {
            if (Recorder.Recording)
            {
                return;
            }

            CumulativeTimestamp = Recorder.ResetCumulativeTimestamp();
        }

        private void DoClearAction()
        {
            Recorder.ClearExistingParts();
            CumulativeTimestamp = Recorder.ResetCumulativeTimestamp();
            OnPropertyChanged(nameof(ShowClear));
        }

        private void DoRecordAction()
        {
            Recorder.GetRecordingMethod().Invoke();
            //OnPropertyChanged(nameof(RecordCommand));
        }

        protected override void OnPropertyChanged(string propertyName = null)
        {
            base.OnPropertyChanged(propertyName);
            base.OnPropertyChanged(nameof(RecordCommand));
            base.OnPropertyChanged(nameof(RecordButtonBackground));
            base.OnPropertyChanged(nameof(RecordButtonForeground));
            base.OnPropertyChanged(nameof(RecordButtonLabel));
            base.OnPropertyChanged(nameof(RecordCaption));
            base.OnPropertyChanged(nameof(ClearCommand));
            base.OnPropertyChanged(nameof(ShowClear));
            base.OnPropertyChanged(nameof(ShowPartTimestamp));
        }


    }
}