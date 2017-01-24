using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

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
