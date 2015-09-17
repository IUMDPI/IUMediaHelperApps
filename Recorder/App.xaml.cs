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

            var objectModel = new ObjectModel(programSettings)
            {
                Part = 1,
                FileUse = "pres"
            };

            var recorder = new RecordingEngine(programSettings, objectModel);
            var combiner = new CombiningEngine(programSettings, objectModel);

            var viewModel = new UserControlsViewModel(programSettings, objectModel, recorder, combiner);

            var userControls = new UserControls
            {
                DataContext = viewModel,
            };

            userControls.Show();
        }
    }
}
