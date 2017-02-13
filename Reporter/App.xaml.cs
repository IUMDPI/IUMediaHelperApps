using System.Configuration;
using System.Windows;
using Common.UserInterface.ViewModels;
using Reporter.Models;
using Reporter.Utilities;

namespace Reporter
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private void InitializeApplication(object sender, StartupEventArgs e)
        {
            var settings = new ProgramSettings(ConfigurationManager.AppSettings);
            var viewModel = new ViewModel(settings, new ReportReader(settings), new LogPanelViewModel());
            var reportWindow = new ReporterWindow
            {
                DataContext = viewModel
            };

            reportWindow.Show();
        }

        private void ApplicationExitHandler(object sender, ExitEventArgs e)
        {
            
        }
    }
}
