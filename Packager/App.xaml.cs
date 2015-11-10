using System.Collections.Generic;
using System.Configuration;
using System.Windows;
using NLog.Config;
using Packager.Engine;
using Packager.Models;
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

            // initialize program settings
            var programSettings = new ProgramSettings(ConfigurationManager.AppSettings);

            // create the view model
            var viewModel = new ViewModel();

            // create the window
            var window = new OutputWindow();


            // initialize the view model with the window
            viewModel.Initialize(window, programSettings);

            // initialize dependency provider
            var dependencyProvider = new DefaultDependencyProvider(programSettings);

            AddObservers(dependencyProvider, viewModel);

            // initialize processors
            var processors = new Dictionary<string, IProcessor>
            {
                {".wav", new AudioProcessor(dependencyProvider)},
                {".mkv", new VideoProcessor(dependencyProvider)}
            };

            // initialize engine
            var engine = new StandardEngine(processors, dependencyProvider);

            // start the engine
            await engine.Start();
        }

        private static void AddObservers(IDependencyProvider dependencyProvider, ViewModel viewModel)
        {
            dependencyProvider.Observers.Add(new GeneralNLogObserver(dependencyProvider.ProgramSettings.LogDirectoryName));

            dependencyProvider.Observers.Add(new ViewModelObserver(viewModel));

            dependencyProvider.Observers.Add(new IssueEmailerObserver(
                dependencyProvider.ProgramSettings,
                dependencyProvider.SystemInfoProvider,
                dependencyProvider.EmailSender));
        }

        private static void ConfigureNLog()
        {
            ConfigurationItemFactory.Default.LayoutRenderers.RegisterDefinition("LogDirectoryName", typeof (LoggingDirectoryLayoutRenderer));
            ConfigurationItemFactory.Default.LayoutRenderers.RegisterDefinition("ProcessingDirectoryName", typeof (ProcessingDirectoryNameLayoutRenderer));
            ConfigurationItemFactory.Default.LayoutRenderers.RegisterDefinition("Barcode", typeof (BarcodeLayoutRenderer));
            ConfigurationItemFactory.Default.LayoutRenderers.RegisterDefinition("ProjectCode", typeof (ProjectCodeLayoutRenderer));
        }
    }
}