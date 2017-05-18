using System;
using System.Threading;
using System.Windows;
using Packager.Engine;
using Packager.Exceptions;
using Packager.Models.ProgramArgumentsModels;
using Packager.Models.SettingsModels;
using Packager.Observers;

namespace Packager.UserInterface
{
    /// <summary>
    ///     Interaction logic for OutputWindow.xaml
    /// </summary>
    public partial class OutputWindow : Window
    {
        private IViewModel ViewModel { get; }
        private IProgramSettings ProgramSettings { get; }
        private IEngine Engine { get; }
        private CancellationTokenSource CancellationTokenSource { get; }
        private IObserverCollection Observers { get; }
        private IProgramArguments ProgramArguments { get; }

        public OutputWindow(IProgramSettings programSettings, IViewModel viewModel, IEngine engine, IObserverCollection observers, IProgramArguments arguments, CancellationTokenSource cancellationTokenSource)
        {
            InitializeComponent();

            ProgramSettings = programSettings;
            ProgramArguments = arguments;
            Engine = engine;
            CancellationTokenSource = cancellationTokenSource;
            ViewModel = viewModel;
            DataContext = viewModel;
            Observers = observers;
        }

        private async void OnWindowLoaded(object sender, RoutedEventArgs e)
        {
            try
            {
                ViewModel.Initialize(this, ProgramSettings.ProjectCode);
                await Engine.Start(CancellationTokenSource.Token);
            }
            catch (Exception exception)
            {
                Observers.LogEngineIssue(exception);
                if (ProgramArguments.Interactive == false)
                {
                    throw new LoggedException(exception);
                }
            }
            
            
        }
    }
}