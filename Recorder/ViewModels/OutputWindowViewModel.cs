using System.ComponentModel;
using System.Configuration;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using JetBrains.Annotations;
using Recorder.Extensions;

namespace Recorder.ViewModels
{
    public class OutputWindowViewModel : INotifyPropertyChanged
    {
        private bool _clear;
        private string _text;
        private Visibility _visibility;
        private bool _autoScroll;
        private ScrollViewer _scrollViewer;

        public OutputWindowViewModel()
        {
            AutoScroll = true;
        }

        public bool AutoScroll
        {
            get { return _autoScroll; }
            set
            {
                _autoScroll = value;
                OnPropertyChanged();
            }
        }

        public string Text
        {
            get { return _text; }
            set
            {
                _text = value;
                ScrollToEnd();
                OnPropertyChanged();
            }
        }


        public void ScrollToEnd()
        {
            if (_scrollViewer == null || AutoScroll == false)
            {
                return;
            }

            if (_scrollViewer.Dispatcher.CheckAccess()==false)
            {
                _scrollViewer.Dispatcher.Invoke(ScrollToEnd);
                return;
            }

            _scrollViewer.ScrollToEnd();
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
            set { _visibility = value; OnPropertyChanged(); }
        }

        public event PropertyChangedEventHandler PropertyChanged;


        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }


        public void StartOutput(string header)
        {
            if (Clear)
            {
                Text = string.Empty;
            }

            Text += $"{header}\n\n";
        }

        public void HookEvents(OutputWindow client)
        {
            client.Closing += OnWindowClosing;
            _scrollViewer = client.OutputText.FindDescendant<ScrollViewer>();
            if (_scrollViewer != null)
            {
                _scrollViewer.ScrollChanged += ScrollChangedHandler;
            }
        }

        private void OnWindowClosing(object sender, CancelEventArgs args)
        {
            Visibility = Visibility.Hidden;
            args.Cancel = true;

        }
        
        private void ScrollChangedHandler(object sender, ScrollChangedEventArgs e)
        {
            var viewer = sender as ScrollViewer;
            if (viewer == null)
            {
                return;
            }

            if (!e.ExtentHeightChange.Equals(0))
            {
                return;
            }

            AutoScroll = viewer.VerticalOffset.Equals(viewer.ScrollableHeight);
        }
    }
}