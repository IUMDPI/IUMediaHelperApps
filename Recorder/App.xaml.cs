using System.Configuration;
using System.Windows;
using Recorder.Models;
using Recorder.Utilities;
using Recorder.ViewModels;

namespace Recorder
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private void InitializeApplication(object sender, StartupEventArgs e)
        {
            var programSettings = new ProgramSettings(ConfigurationManager.AppSettings);

            var recorder = new RecordingEngine(programSettings)
            {
                Part = 1,
                FileUse = "pres"
            };

            var viewModel = new UserControlsViewModel(programSettings, recorder);

            var userControls = new UserControls
            {
                DataContext = viewModel,
            };

            userControls.Show();
        }
    }
}
