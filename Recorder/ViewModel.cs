using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using JetBrains.Annotations;
using Recorder.Models;

namespace Recorder
{
    public class ViewModel : INotifyPropertyChanged
    {
        private readonly IProgramSettings _settings;
        private string _barcode;
        private int _part;
        private PauseCommand _pauseButtonCommand;

        private RecordCommand _recordButtonCommand;
        private bool _recording;
        private bool _paused;

        public ViewModel(IProgramSettings settings)
        {
            _settings = settings;
           
            RecordButtonCommand = new RecordCommand(this);
            PauseButtonCommand = new PauseCommand(this);
            
        }

        public RecordCommand RecordButtonCommand
        {
            get { return _recordButtonCommand; }
            set
            {
                _recordButtonCommand = value;
                OnPropertyChanged();
            }
        }

        public PauseCommand PauseButtonCommand
        {
            get { return _pauseButtonCommand; }
            set
            {
                _pauseButtonCommand = value;
                OnPropertyChanged();
            }
        }

        public string RecordingButtonLabel => Recording ? "<" : "=";

        public bool Recording
        {
            get { return _recording; }
            set
            {
                _recording = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(RecordingButtonLabel));
            }
        }

        public bool Paused
        {
            get { return _paused; }
            set { _paused = value; OnPropertyChanged(); }
        }

        public string BarCode
        {
            get { return _barcode; }
            set
            {
                _barcode = value;
                OnPropertyChanged();
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
            //PauseButtonCommand.CanExecuteChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }


    public class PauseCommand : ICommand, INotifyPropertyChanged
    {

        private readonly ViewModel _viewModel;

        public PauseCommand(ViewModel viewModel)
        {
            _viewModel = viewModel;
        }

        public bool CanExecute(object parameter)
        {
            return _viewModel.Recording;
        }

        public void Execute(object parameter)
        {
            _viewModel.Paused = !_viewModel.Paused;
        }

        public event EventHandler CanExecuteChanged;
        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
    public class RecordCommand : ICommand
    {
        private readonly ViewModel _viewModel;

        public RecordCommand(ViewModel viewModel)
        {
            _viewModel = viewModel;
        }

        public bool CanExecute(object parameter)
        {
            return _viewModel.Paused == false;
        }

        public void Execute(object parameter)
        {
            _viewModel.Recording = !_viewModel.Recording;
        }

        public event EventHandler CanExecuteChanged;
    }
}