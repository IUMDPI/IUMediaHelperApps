using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows;
using JetBrains.Annotations;

namespace Recorder.ViewModels
{
    public class OutputWindowViewModel : INotifyPropertyChanged, IClosing
    {
        private bool _clear;

        private string _text;
        private Visibility _visibility;
       
        public OutputWindowViewModel()
        {
            VolumeMeterViewModel = new VolumeMeterViewModel();
            FrameStatsViewModel = new FrameStatsViewModel();
        }

        public VolumeMeterViewModel VolumeMeterViewModel { get; }
        public FrameStatsViewModel FrameStatsViewModel { get; }

        public string Text
        {
            get { return _text; }
            set
            {
                _text = value;
                OnPropertyChanged();
            }
        }
        
        public void ShowVolumeMeter()
        {
            VolumeMeterViewModel.VolumeMeterVisibility = Visibility.Visible;
        }

        public void HideVolumeMeter()
        {
            VolumeMeterViewModel.VolumeMeterVisibility = Visibility.Collapsed;
        }

        public bool Clear
        {
            get { return _clear; }
            set
            {
                _clear = value;
                OnPropertyChanged();
            }
        }

        public Visibility Visibility
        {
            get { return _visibility; }
            set
            {
                _visibility = value;
                OnPropertyChanged();
            }
        }

        public bool CancelWindowClose()
        {
            Visibility = Visibility.Hidden;
            return true;
        }

        public event PropertyChangedEventHandler PropertyChanged;


        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public void StartOutput(string header)
        {
            Visibility = Visibility.Visible;
            Text += $"{header}\n\n";
        }

        public void WriteLine(string text)
        {
            var builder = new StringBuilder(Text);
            builder.AppendLine(text);
            Text = builder.ToString();
        }
    }
}