using System;
using System.Windows;
using Recorder.ViewModels;

namespace Recorder
{
    /// <summary>
    ///     Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class UserControls : Window
    {
        public UserControls()
        {
            InitializeComponent();
        }

        protected override void OnSourceInitialized(EventArgs e)
        {
            var viewModel = DataContext as UserControlsViewModel;
            if (viewModel == null)
            {
                return;
            }

            viewModel.HookEvents(this);
            base.OnSourceInitialized(e);
        }
    }
}