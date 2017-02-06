using System.Windows;

namespace Reporter
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class ReporterWindow : Window
    {
        public ReporterWindow()
        {
            InitializeComponent();
        }

        private async void OnLoadedHandler(object sender, RoutedEventArgs e)
        {
            var viewModel = DataContext as ViewModel;
            if (viewModel == null)
            {
                return;
            }

            await viewModel.Initialize(ReportText);
        }
    }
}
