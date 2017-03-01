using System.Collections.Generic;
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
            var logPanelViewModel = new LogPanelViewModel();
            var settings = new ProgramSettings(ConfigurationManager.AppSettings);
            var reportReaders = new List<IReportRenderer>()
            {
                new FileReportRenderer(settings, logPanelViewModel),
                new TaskSchedulerRenderer(logPanelViewModel)
            };

            
            var viewModel = new ViewModel(settings, reportReaders, logPanelViewModel);
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
