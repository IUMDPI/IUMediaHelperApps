using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using Recorder.Utilities;

namespace Recorder.ViewModels
{
    public class BarcodePanelViewModel : AbstractPanelViewModel
    {
        private ICommand _nextCommand;

        public BarcodePanelViewModel(UserControlsViewModel parent, RecordingEngine recorder, string projectCode) : base(parent, recorder)
        {
            ProjectCode = projectCode;
            Part = 1;
            FileUse = FileUses.First().Item2;
            Touched = false;
        }

        private string ProjectCode { get; }

        public string Filename => Recorder.Filename;

        public string FileUse
        {
            get { return Recorder.FileUse; }
            set
            {
                FlagTouched(Recorder.FileUse, value);
                Recorder.FileUse = value;
                OnPropertyChanged();
            }
        }

        public string FilenameIssue => Recorder.FilePartsValid().IsValid == false
            ? Recorder.FilePartsValid().ErrorContent.ToString()
            : string.Empty;

        public List<Tuple<string, string>> FileUses => Recorder.FileUses;

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
            get { return Recorder.Barcode; }
            set
            {
                FlagTouched(Recorder.Barcode, value);
                Recorder.Barcode = value;
                OnPropertyChanged();
            }
        }

        public int Part
        {
            get { return Recorder.Part; }
            set
            {
                FlagTouched(Recorder.Part, value);
                Recorder.Part = value;
                OnPropertyChanged();
            }
        }

        protected override void OnPropertyChanged(string propertyName = null)
        {
            base.OnPropertyChanged(propertyName);
            base.OnPropertyChanged(nameof(Filename));
            base.OnPropertyChanged(nameof(FilenameIssue));
        }
    }
}