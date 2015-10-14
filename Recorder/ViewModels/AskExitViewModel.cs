using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using JetBrains.Annotations;

namespace Recorder.ViewModels
{
    public class AskExitViewModel : AbstractNotifyModel, IClosing
    {
       
        private bool _exitNow;

        public AskExitViewModel()
        {
            YesText = "Yes";
            NoText = "No";
            Question = "Stop recording and exit?";
            YesCommand = new RelayCommand(
                param => DoExit());
            NoCommand = new RelayCommand(
                param => ShowPanel = false);

        }


        public bool CancelWindowClose()
        {
            FlashPanel = false;

            if (_exitNow)
            {
                return false;
            }

            ShowPanel = true;
            FlashPanel = true;

            return true;
        }

        private void DoExit()
        {
            _exitNow = true;
            Application.Current.MainWindow.Close();
        }
    }
}