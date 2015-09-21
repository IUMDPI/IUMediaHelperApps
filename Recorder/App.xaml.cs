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
        private RecordingEngine _recorder;
        private CombiningEngine _combiner;

        private void InitializeApplication(object sender, StartupEventArgs e)
        {
            var programSettings = new ProgramSettings(ConfigurationManager.AppSettings);

            var objectModel = new ObjectModel(programSettings)
            {
                Part = 1,
                FileUse = "pres"
            };

            _recorder = new RecordingEngine(programSettings, objectModel);
            _combiner = new CombiningEngine(programSettings, objectModel);

            var viewModel = new UserControlsViewModel(programSettings, objectModel, _recorder, _combiner);

            var userControls = new UserControls
            {
                DataContext = viewModel,
            };

            userControls.Closing += viewModel.OnWindowClosing;
            userControls.Show();
        }

        private void ApplicationExitHandler(object sender, ExitEventArgs e)
        {
            _recorder?.Dispose();
            _combiner?.Dispose();
        }
    }
}
