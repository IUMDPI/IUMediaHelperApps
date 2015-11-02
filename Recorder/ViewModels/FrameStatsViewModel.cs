using System.ComponentModel;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;

namespace Recorder.ViewModels
{
    public class FrameStatsViewModel : INotifyPropertyChanged
    {
        private string _currentFrame;
        private string _droppedFrames;

        public FrameStatsViewModel()
        {
            Reset();
        }

        private string _duplicateFrames;

        public string CurrentFrame
        {
            get { return _currentFrame; }
            set { _currentFrame = value; OnPropertyChanged(); }
        }

        public string DroppedFrames
        {
            get { return _droppedFrames; }
            set { _droppedFrames = value; OnPropertyChanged(); }
        }

        public string DuplicateFrames
        {
            get { return _duplicateFrames; }
            set { _duplicateFrames = value; OnPropertyChanged(); }
        }

        public void Reset()
        {
            CurrentFrame = "0";
            DroppedFrames = "0";
            DuplicateFrames = "0";
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}