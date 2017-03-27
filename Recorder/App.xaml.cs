using System.Configuration;
using System.Windows;
using System.Windows.Threading;
using Common.Models;
using Recorder.Dialogs;
using Recorder.Exceptions;
using Recorder.Models;
using Recorder.Utilities;
using Recorder.ViewModels;

namespace Recorder
{
    /// <summary>
    ///     Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private OutputWindow _outputWindow;
        private UserControls _userControls;
        private UserControlsViewModel _viewModel;
       
        public App()
        {
            Dispatcher.UnhandledException += OnDispatcherUnhandledException;
        }

        private void InitializeApplication(object sender, StartupEventArgs e)
        {
            var programSettings = new ProgramSettings(ConfigurationManager.AppSettings);
          
            var objectModel = new ObjectModel(programSettings)
            {
                Part = 1,
                FileUse = FileUsages.PreservationMaster
            };

            _viewModel = new UserControlsViewModel(programSettings, objectModel);
            
            ConfigureWindows();
            ShowWindows();

            programSettings.Verify();
        }

        private void ConfigureWindows()
        {
            _userControls = new UserControls
            {
                DataContext = _viewModel
            };

            _outputWindow = new OutputWindow
            {
                DataContext = _viewModel.OutputWindowViewModel
            };
        }

        private void ShowWindows()
        {
            _userControls.Show();
            _outputWindow.Owner = _userControls;

            // initially position output window next to main window
            _outputWindow.Left = _userControls.Left + _userControls.Width;
            _outputWindow.Width = _userControls.Width;
            _outputWindow.Height = _userControls.Height;
            _outputWindow.Top = _userControls.Top;

            _outputWindow.Show();
        }

        private void ApplicationExitHandler(object sender, ExitEventArgs e)
        {
            _viewModel?.Dispose();
        }

        private void OnDispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            var errorMessage = e.Exception is AbstractHandledException ?
                e.Exception.Message:
                $"An unhandled exception occurred: {e.Exception.Message}";

            if (_viewModel?.NotifyIssueModel == null)
            {
                MessageBox.Show(errorMessage, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else { _viewModel.NotifyIssueModel.Notify(errorMessage);}
            
            e.Handled = true;
        }
    }
}