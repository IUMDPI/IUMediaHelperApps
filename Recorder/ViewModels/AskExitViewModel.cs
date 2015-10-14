using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using JetBrains.Annotations;

namespace Recorder.ViewModels
{
    public class AskExitViewModel:INotifyPropertyChanged, IClosing
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private bool _askExit;
        private ICommand _exitCommand;
        private ICommand _cancelExit;

        private bool _exitNow;
        private bool _flashPanel;

        public bool AskExit
        {
            get { return _askExit; }
            set { _askExit = value; OnPropertyChanged(); }
        }

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public bool FlashPanel
        {
            get { return _flashPanel;}
            set { _flashPanel = value; OnPropertyChanged(); }
        }

        public string YesText => "Yes";

        public string NoText => "No";

        public bool CancelWindowClose()
        {
            FlashPanel = false;

            if (_exitNow)
            {
                return false;
            }

            AskExit = true;
            FlashPanel = true;
            
            return true;
        }

        public string Question=>"Stop recording and exit?";

        public ICommand YesCommand
        {
            get
            {
                return _exitCommand ?? (_exitCommand = new RelayCommand(
                    param => DoExit()));
            }
        }
        public ICommand NoCommand
        {
            get
            {
                return _cancelExit ?? (_cancelExit = new RelayCommand(
                    param => AskExit = false));
            }
        }
        
        private void DoExit()
        {
            _exitNow = true;
            Application.Current.MainWindow.Close();
        }
    }
}
