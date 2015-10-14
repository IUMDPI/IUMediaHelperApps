using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using Recorder.Extensions;
using Recorder.ViewModels;

namespace Recorder
{
    /// <summary>
    ///     Interaction logic for OutputWindow.xaml
    /// </summary>
    public partial class OutputWindow : Window
    {
        private bool _autoScroll;
        private ScrollViewer _scrollViewer;

        public OutputWindow()
        {
            InitializeComponent();
            _autoScroll = true;
        }

        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);

            _scrollViewer = OutputText.FindDescendant<ScrollViewer>();
            if (_scrollViewer != null)
            {
                _scrollViewer.ScrollChanged += ScrollChangedHandler;
            }
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

            _autoScroll = viewer.VerticalOffset.Equals(viewer.ScrollableHeight);
        }

        private void OnOutputTextChanged(object sender, TextChangedEventArgs e)
        {
            ScrollToEnd();
        }

        private void ScrollToEnd()
        {
            if (_scrollViewer == null || _autoScroll == false)
            {
                return;
            }

            if (_scrollViewer.Dispatcher.CheckAccess() == false)
            {
                _scrollViewer.Dispatcher.Invoke(ScrollToEnd);
                return;
            }

            _scrollViewer.ScrollToEnd();
        }

        private void OnWindowClosing(object sender, CancelEventArgs e)
        {
            var onClosing = DataContext as IClosing;
            if (onClosing == null)
            {
                return;
            }

            e.Cancel = onClosing.CancelWindowClose();
        }
    }
}