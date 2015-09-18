using System.Windows.Input;
using Recorder.Models;
using Recorder.Utilities;

namespace Recorder.ViewModels
{
    public class FinishPanelViewModel : AbstractPanelViewModel
    {
        private CombiningEngine Combiner { get; set; }
        private ICommand _backCommand;
        private ICommand _combineCommand;

        public FinishPanelViewModel(UserControlsViewModel parent, ObjectModel objectModel, CombiningEngine combiner) : base(parent, objectModel)
        {
            Combiner = combiner;
        }

        public override void Initialize()
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
        
        public ICommand CombineCommand
        {
            get
            {
                return _combineCommand
                       ?? (_combineCommand = new RelayCommand(param => DoCombine(), param=>Combiner.OkToCombine().IsValid));
            }
        }

        private void DoCombine()
        {
            Combiner.Combine();
            OnPropertyChanged(nameof(CombineCommand));
        }
    }
}