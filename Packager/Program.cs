using System;
using System.Collections.Generic;
using System.Configuration;
using System.Windows.Forms;
using Packager.Engine;
using Packager.Models;
using Packager.Observers;
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

            // initialize program settings
            var programSettings = new ProgramSettings(ConfigurationManager.AppSettings);

            // initialize observers
            var observers = new List<IObserver>();

            // initialize utility provider
            var dependencyProvider = new DefaultDependencyProvider(programSettings, observers);
            // initialize processors
            var processors = new Dictionary<string, IProcessor>
            {
                {".wav", new AudioProcessor(dependencyProvider)}
            };

            // initialize engine
            var engine = new StandardEngine(processors, dependencyProvider);

            // pass engine into output form
            var outputForm = new OutputForm(engine);

            // load and run output form
            Application.Run(outputForm);
        }
    }
}