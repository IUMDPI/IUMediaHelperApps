using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading;
using Common.UserInterface.Commands;
using Common.UserInterface.ViewModels;
using Packager.Annotations;

namespace Packager.UserInterface
{
    public class ViewModel : INotifyPropertyChanged, IViewModel, IDisposable
    {
        public ILogPanelViewModel LogPanelViewModel { get; }
        private readonly CancellationTokenSource _cancellationTokenSource;
        private RelayCommand _cancelCommand;
        private bool _processing;
        private string _processingMessage;
        private string _title;


        public ViewModel(ILogPanelViewModel logPanelViewModel, CancellationTokenSource cancellationTokenSource)
        {
            LogPanelViewModel = logPanelViewModel;
            _cancellationTokenSource = cancellationTokenSource;
        }

        public string Title
        {
            get { return _title; }
            set
            {
                _title = value;
                OnPropertyChanged();
            }
        }

        public RelayCommand CancelCommand
        {
            get
            {
                return _cancelCommand ?? (_cancelCommand = new RelayCommand(param => DoCancel(), param => Processing));
            }
        }

        public void Dispose()
        {
            _cancellationTokenSource?.Dispose();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public string ProcessingMessage
        {
            get { return _processingMessage; }
            set
            {
                _processingMessage = value;
                OnPropertyChanged();
            }
        }

        public bool Processing
        {
            get { return _processing; }
            set
            {
                _processing = value;
                OnPropertyChanged();
                CancelCommand?.RaiseCanExecuteChanged();
            }
        }

        public void Initialize(OutputWindow outputWindow, string projectCode)
        {
            LogPanelViewModel.Initialize(outputWindow.OutputText);
            Title = $"{projectCode.ToUpperInvariant()} Media Packager";
        }

        private void DoCancel()
        {
            try
            {
                _cancellationTokenSource.Cancel(true);
            }
            catch (OperationCanceledException)
            {
                // swallow issue here - it will be reported elsewhere
            }
        }

        [NotifyPropertyChangedInvocator]
        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            var handler = PropertyChanged;
            handler?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}