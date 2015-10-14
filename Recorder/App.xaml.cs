using System.Configuration;
using System.Windows;
using Recorder.Models;
using Recorder.ViewModels;

namespace Recorder
{
    /// <summary>
    ///     Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private Dialogs.OutputWindow _outputWindow;
        private Dialogs.UserControls _userControls;
        private UserControlsViewModel _viewModel;

        private void InitializeApplication(object sender, StartupEventArgs e)
        {
            var programSettings = new ProgramSettings(ConfigurationManager.AppSettings);

            var objectModel = new ObjectModel(programSettings)
            {
                Part = 1,
                FileUse = "pres"
            };

            _viewModel = new UserControlsViewModel(programSettings, objectModel);

            ConfigureWindows();
            ShowWindows();
        }

        private void ConfigureWindows()
        {
            _userControls = new Dialogs.UserControls
            {
                DataContext = _viewModel
            };

            _outputWindow = new Dialogs.OutputWindow
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
    }
}