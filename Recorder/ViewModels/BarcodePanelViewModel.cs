using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using Recorder.Models;

namespace Recorder.ViewModels
{
    public class BarcodePanelViewModel : AbstractPanelViewModel
    {
        private ICommand _nextCommand;

        public BarcodePanelViewModel(UserControlsViewModel parent, ObjectModel objectModel, string projectCode) : base(parent, objectModel)
        {
            ProjectCode = projectCode;
            Part = 1;
            FileUse = FileUses.First().Item2;
            Touched = false;
            ActionButton = new ActionButtonModel
            {
                ButtonCaption = "1",
                LabelCaption = "Start",
                Scale = 1,
                ButtonCommand = new RelayCommand(param => parent.ShowPanel<BarcodePanelViewModel>())
            };
        }

        private string ProjectCode { get; }

        public string Filename => ObjectModel.Filename;

        public string FileUse
        {
            get { return ObjectModel.FileUse; }
            set
            {
                FlagTouched(ObjectModel.FileUse, value);
                ObjectModel.FileUse = value;
                OnPropertyChanged();
            }
        }

        public string FilenameIssue => ObjectModel.FilePartsValid().IsValid == false
            ? ObjectModel.FilePartsValid().ErrorContent.ToString()
            : string.Empty;

        public List<Tuple<string, string>> FileUses => ObjectModel.FileUses;

        public override bool IsEnabled => (Recorder.Recording == false);

        public override string BackButtonText => "";
        public override string NextButtonText => "Start recording";
        public override string Instructions => "Enter the object's barcode and its part. These will be used to generate the filename.";

        public override ICommand NextCommand
        {
            get
            {
                return _nextCommand
                       ?? (_nextCommand = new RelayCommand(param => Parent.ShowPanel<RecordPanelViewModel>()));
            }
        }

        public override ICommand BackCommand => null;

        public string Barcode
        {
            get { return ObjectModel.Barcode; }
            set
            {
                FlagTouched(ObjectModel.Barcode, value);
                ObjectModel.Barcode = value;
                OnPropertyChanged();
            }
        }

        public int Part
        {
            get { return ObjectModel.Part; }
            set
            {
                FlagTouched(ObjectModel.Part, value);
                ObjectModel.Part = value;
                OnPropertyChanged();
            }
        }

        public override void Initialize()
        {
        }

        protected override void OnPropertyChanged(string propertyName = null)
        {
            base.OnPropertyChanged(propertyName);
            base.OnPropertyChanged(nameof(Filename));
            base.OnPropertyChanged(nameof(FilenameIssue));
        }
    }
}