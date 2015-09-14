using System.Configuration;
using System.Windows;
using Recorder.Models;

namespace Recorder
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            var programSettings = new ProgramSettings(ConfigurationManager.AppSettings);

            var viewModel = new ViewModel(programSettings);

            var userControls = new UserControls {DataContext = viewModel};

            userControls.Show();
        }
    }
}
