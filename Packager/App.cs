using System.Collections.Generic;
using System.Configuration;
using System.Windows;
using NLog.Config;
using Packager.Engine;
using Packager.Factories;
using Packager.Observers;
using Packager.Observers.LayoutRenderers;
using Packager.Processors;
using Packager.Providers;
using Packager.UserInterface;

namespace Packager
{
    /// <summary>
    ///     Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override async void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            ConfigureNLog();
        
            // initialize dependency provider with program settings
            var dependencyProvider = new DefaultDependencyProvider(
                SettingsFactory.Import(ConfigurationManager.AppSettings));

            // create the view model
            var viewModel = new ViewModel(dependencyProvider.CancellationTokenSource);

            // create the window
            var window = new OutputWindow();

            // initialize the view model with the window and 
            // the program settings project code (used for the window title)
            viewModel.Initialize(window, dependencyProvider.ProgramSettings.ProjectCode);

            AddObservers(dependencyProvider, viewModel);

            // initialize processors
            var processors = new Dictionary<string, IProcessor>
            {
                {".wav", new AudioProcessor(dependencyProvider)},
                {".mkv", new VideoProcessor(dependencyProvider)}
            };

            // initialize engine
            var engine = new StandardEngine(processors, dependencyProvider, viewModel);

            // start the engine
            await engine.Start(dependencyProvider.CancellationTokenSource.Token);
        }

        private static void AddObservers(IDependencyProvider dependencyProvider, ViewModel viewModel)
        {
            dependencyProvider.Observers.Add(new GeneralNLogObserver(dependencyProvider.ProgramSettings));
            dependencyProvider.Observers.Add(new ObjectNLogObserver(dependencyProvider.ProgramSettings));

            dependencyProvider.Observers.Add(new ViewModelObserver(viewModel));
            dependencyProvider.Observers.Add(new IssueEmailerObserver(
                dependencyProvider.ProgramSettings,
                dependencyProvider.SystemInfoProvider,
                dependencyProvider.EmailSender));
        }

        private static void ConfigureNLog()
        {
            ConfigurationItemFactory.Default.LayoutRenderers.RegisterDefinition("ProjectCode",
                typeof(ProjectCodeLayoutRenderer));
            ConfigurationItemFactory.Default.LayoutRenderers.RegisterDefinition("LogDirectoryName",
                typeof (LoggingDirectoryLayoutRenderer));
            ConfigurationItemFactory.Default.LayoutRenderers.RegisterDefinition("Barcode",
                typeof (BarcodeLayoutRenderer));
            
        }
    }
}