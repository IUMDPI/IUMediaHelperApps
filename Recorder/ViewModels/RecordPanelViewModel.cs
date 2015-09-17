using System;
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
        private delegate void SetTimestamp(TimeSpan timestamp);

        private ICommand _backCommand;
        private ICommand _clearCommand;
        private ICommand _nextCommand;
        private ICommand _recordCommand;
        private string _timestamp;

        public RecordPanelViewModel(UserControlsViewModel parent, ObjectModel objectModel, RecordingEngine recorder) : base(parent, objectModel)
        {
            Recorder = recorder;
            Recorder.InitializeTimestampDelegate( ChangeTimestamp);
        }

        private RecordingEngine Recorder { get; set; }

        public bool ShowClear => ObjectModel.PartsPresent();

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

        private void ChangeTimestamp(TimeSpan timestamp)
        {
            if (!Dispatcher.CurrentDispatcher.CheckAccess())
            {
                Dispatcher.CurrentDispatcher.Invoke(() => ChangeTimestamp(timestamp));
                return;
            }

            Timestamp = timestamp.ToString();
        }

        public string Timestamp
        {
            get { return _timestamp; }
            set { _timestamp = value; Debug.WriteLine(value); OnPropertyChanged(); }
        }

        public override string BackButtonText => "Create new object";
        public override string NextButtonText => "Generate single file";

        public override string Instructions
            => "Click record to start recording. You can stop and restart your recording as often as needed, and the parts will be assembled into a single file in the Finish step.";

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

        private void DoClearAction()
        {
            Recorder.ClearExistingParts();
            OnPropertyChanged(nameof(ShowClear));
        }

        private void DoRecordAction()
        {
            Recorder.GetRecordingMethod().Invoke();
            OnPropertyChanged(nameof(RecordCommand));
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
        }
    }
}