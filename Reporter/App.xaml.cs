using System.Windows;
using Common.UserInterface.ViewModels;
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
            var viewModel = new ViewModel(new ReportReader(), new LogPanelViewModel());
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
