using System.Windows.Input;
using Recorder.Utilities;

namespace Recorder.ViewModels
{
    public class FinishPanelViewModel : AbstractPanelViewModel
    {
        private ICommand _backCommand;

        public FinishPanelViewModel(UserControlsViewModel parent, RecordingEngine recorder) : base(parent, recorder)
        {
        }

        public override string BackButtonText => "Continue recording";
        public override string NextButtonText => "";
        public override string Instructions => "Click finalize to generate your file and move it to the output folder";
        public override ICommand NextCommand => null;

        public override ICommand BackCommand
        {
            get
            {
                return _backCommand
                       ?? (_backCommand = new RelayCommand(param => Parent.ShowPanel<RecordPanelViewModel>()));
            }
        }
    }
}