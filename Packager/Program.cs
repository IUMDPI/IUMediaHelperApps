using System;
using System.Collections.Generic;
using System.Configuration;
using System.Windows.Forms;
using Packager.Engine;
using Packager.Models;
using Packager.Observers;
using Packager.Processors;
using Packager.Providers;
using Packager.Utilities;

namespace Packager
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            // initialize program settings
            var programSettings = new ProgramSettings(ConfigurationManager.AppSettings);

            // initialize utility provider
            var utilityProvider = new DefaultDependencyProvider();
            
            // initialize observers
            var observers = new List<IObserver>();

            // initialize processors
            var processors = new Dictionary<string, IProcessor>
            {
                {".wav", new AudioProcessor(programSettings, utilityProvider, observers)},
            };

            // initialize engine
            var engine = new StandardEngine(programSettings, processors, utilityProvider, observers);
            
            // pass engine into output form
            var outputForm = new OutputForm(engine);

            // load and run output form
            Application.Run(outputForm);


        }
    }
}
