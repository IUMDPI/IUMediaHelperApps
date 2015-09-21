using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using JetBrains.Annotations;

namespace Recorder.ViewModels
{
    public class ActionButtonModel : INotifyPropertyChanged
    {
        private string _buttonCaption;
        private ICommand _buttonCommand;
        private string _labelCaption;
        private double _scale;
        private double _opacity;

        public ActionButtonModel()
        {
            Opacity = 1;
            Scale = 1;
        }

        public string LabelCaption
        {
            get { return _labelCaption; }
            set
            {
                _labelCaption = value;
                OnPropertyChanged();
            }
        }

        public string ButtonCaption
        {
            get { return _buttonCaption; }
            set
            {
                _buttonCaption = value;
                OnPropertyChanged();
            }
        }

        public double Opacity
        {
            get { return _opacity; }
            set { _opacity = value; OnPropertyChanged(); }
        }

        public double Scale
        {
            get { return _scale; }
            set
            {
                _scale = value;
                OnPropertyChanged();
            }
        }

        public ICommand ButtonCommand
        {
            get { return _buttonCommand; }
            set
            {
                _buttonCommand = value;
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