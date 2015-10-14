using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using JetBrains.Annotations;

namespace Recorder.ViewModels
{
    public abstract class AbstractNotifyModel : INotifyPropertyChanged
    {
        private ICommand _noCommand;
        private ICommand _yesCommand;
        private string _yesText;
        private string _noText;
        private bool _flashPanel;
        private bool _showPanel;
        private string _question;

        public string YesText
        {
            get { return _yesText; }
            set
            {
                _yesText = value;
                OnPropertyChanged();
            }
        }

        public string NoText
        {
            get { return _noText; }
            set
            {
                _noText = value;
                OnPropertyChanged();
            }
        }
        public string Question
        {
            get { return _question; }
            set { _question = value; OnPropertyChanged(); }
        }

        public ICommand YesCommand
        {
            get { return _yesCommand; }
            set
            {
                _yesCommand = value;
                OnPropertyChanged();
            }
        }

        public ICommand NoCommand
        {
            get { return _noCommand; }
            set
            {
                _noCommand = value;
                OnPropertyChanged();
            }
        }

        public bool FlashPanel
        {
            get { return _flashPanel; }
            set
            {
                _flashPanel = value;
                OnPropertyChanged();
            }
        }

        public bool ShowPanel
        {
            get { return _showPanel; }
            set
            {
                _showPanel = value;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}