using System.ComponentModel;
using System.Configuration;
using System.Runtime.CompilerServices;
using System.Windows;
using JetBrains.Annotations;

namespace Recorder.ViewModels
{
    public class VolumeMeterViewModel : INotifyPropertyChanged
    {
        private float _inputLevel;
        private Visibility _volumeMeterVisibility;

        public VolumeMeterViewModel()
        {
            VolumeMeterVisibility = Visibility.Collapsed;
        }

        public Visibility VolumeMeterVisibility
        {
            get { return _volumeMeterVisibility; }
            set { _volumeMeterVisibility = value; OnPropertyChanged(); }
        }

        public float InputLevel
        {
            get { return _inputLevel; }
            set
            {
                _inputLevel = value;
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