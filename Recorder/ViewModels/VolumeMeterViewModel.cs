using System.ComponentModel;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;

namespace Recorder.ViewModels
{
    public class VolumeMeterViewModel : INotifyPropertyChanged
    {
        private float _inputLevel;

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