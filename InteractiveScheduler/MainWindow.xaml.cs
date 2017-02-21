using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Common.TaskScheduler.Factory;
using InteractiveScheduler.Models;
using InteractiveScheduler.Services;

namespace InteractiveScheduler
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        public MainWindow()
        {
            InitializeComponent();

            MouseDown += MouseDownHandler;

            DataContext = new ViewModel(
                new FileDialogService(this), 
                new UserService(),
                new TaskSchedulerFactory());
        }

        private void MouseDownHandler(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                this.DragMove();
        }

        private void PasswordChangedHandler(object sender, RoutedEventArgs e)
        {
            var viewModel = DataContext as ViewModel;
            if (viewModel == null)
            {
                return;
            }

            var passwordBox = sender as PasswordBox;
            if (passwordBox == null)
            {
                return;
            }

            viewModel.Password = passwordBox.SecurePassword;
        }

        private void FlashingCompleted(object sender, EventArgs e)
        {
            var viewModel = DataContext as ViewModel;
            if (viewModel == null)
            {
                return;
            }
            viewModel.FlashMessage = false;
        }
    }
}
