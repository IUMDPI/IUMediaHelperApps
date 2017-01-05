using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using InteractiveScheduler.Services;

namespace InteractiveScheduler
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        public MainWindow()
        {
            InitializeComponent();

            MouseDown += MouseDownHandler;

            DataContext = new ViewModel(
                new FileDialogService(this), 
                new UserService(),
                new TaskScheduler());
        }

        private void MouseDownHandler(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                this.DragMove();
        }

        private void PasswordChangedHandler(object sender, RoutedEventArgs e)
        {
            var viewModel = DataContext as ViewModel;
            if (viewModel == null)
            {
                return;
            }

            var passwordBox = sender as PasswordBox;
            if (passwordBox == null)
            {
                return;
            }

            viewModel.Password = passwordBox.SecurePassword;
        }
    }
}
