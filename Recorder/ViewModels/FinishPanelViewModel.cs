using System.Windows.Input;
using Recorder.Models;
using Recorder.Utilities;

namespace Recorder.ViewModels
{
    public class FinishPanelViewModel : AbstractPanelViewModel
    {
       
        private ICommand _backCommand;
        private ICommand _combineCommand;

        public FinishPanelViewModel(UserControlsViewModel parent, ObjectModel objectModel) : base(parent, objectModel)
        {
       
            ActionButton = new ActionButtonModel
            {
                ButtonCaption = "3",
                LabelCaption = "Finish",
                Scale = 1,
                ButtonCommand = new RelayCommand(param => parent.ShowPanel<FinishPanelViewModel>())
            };
        }

        public override void Initialize()
        {
        }

        public override bool IsEnabled => Recorder.Recording == false && ObjectModel.PartsPresent();
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