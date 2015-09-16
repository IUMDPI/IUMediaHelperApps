using System.Windows.Input;
using System.Windows.Media;
using Recorder.Utilities;

namespace Recorder.ViewModels
{
    public class RecordPanelViewModel : AbstractPanelViewModel
    {
        private ICommand _backCommand;
        private ICommand _nextCommand;
        private ICommand _recordCommand;

        
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

                return Recorder.PartsPresent()
                    ? "Resume recording"
                    : "Start recording";
            }
        }

        public RecordPanelViewModel(UserControlsViewModel parent, RecordingEngine recorder) : base(parent, recorder)
        {
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
                    new RelayCommand(param=>DoRecordAction(), param=>Recorder.OkToRecord().IsValid));
            }
        }

        private void DoRecordAction()
        {
            Recorder.GetRecordingMethod().Invoke();
            OnPropertyChanged(nameof(RecordCommand));
            OnPropertyChanged(nameof(RecordButtonBackground));
            OnPropertyChanged(nameof(RecordButtonForeground));
            OnPropertyChanged(nameof(RecordButtonLabel));
            OnPropertyChanged(nameof(RecordCaption));
        }
    }
}