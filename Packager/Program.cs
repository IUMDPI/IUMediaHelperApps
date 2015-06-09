using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Windows.Forms;
using NLog;
using NLog.Config;
using Packager.Engine;
using Packager.Models;
using Packager.Observers;
using Packager.Observers.LayoutRenderers;
using Packager.Processors;
using Packager.Providers;

namespace Packager
{
    internal static class Program
    {
        /// <summary>
        ///     The main entry point for the application.
        /// </summary>
        [STAThread]
        private static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            ConfigureNLog();


            // initialize program settings
            var programSettings = new ProgramSettings(ConfigurationManager.AppSettings);
            
            // initialize observers
            var observers = new List<IObserver>
            {
                new GeneralNLogObserver(programSettings.LogDirectoryName, programSettings.ProcessingDirectory)
            };

            // initialize utility provider
            var dependencyProvider = new DefaultDependencyProvider(programSettings, observers);
            // initialize processors
            var processors = new Dictionary<string, Type>
            {
                {".wav", typeof(AudioProcessor)}
            };

            // initialize engine
            var engine = new StandardEngine(processors, dependencyProvider);

            // pass engine into output form
            var outputForm = new OutputForm(engine);

            // load and run output form
            Application.Run(outputForm);
        }
        
        private static void ConfigureNLog()
        {
            ConfigurationItemFactory.Default.LayoutRenderers.RegisterDefinition("LogDirectoryName", typeof(LoggingDirectoryLayoutRenderer));
            ConfigurationItemFactory.Default.LayoutRenderers.RegisterDefinition("ProcessingDirectoryName", typeof(ProcessingDirectoryNameLayoutRenderer));
            ConfigurationItemFactory.Default.LayoutRenderers.RegisterDefinition("Barcode", typeof(BarcodeLayoutRenderer));
            ConfigurationItemFactory.Default.LayoutRenderers.RegisterDefinition("ProjectCode", typeof(ProjectCodeLayoutRenderer));
        }


    }
}