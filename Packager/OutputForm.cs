using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Packager.Extensions;
using Packager.Models;
using Packager.Observers;
using Packager.Utilities;

namespace Packager
{
    public partial class OutputForm : Form
    {
        private readonly Dictionary<string, IProcessor> _processors;
        private readonly List<IObserver> _observers;
        private readonly IProgramSettings _programSettings;
        private readonly IUtilityProvider _utilityProvider;

        public OutputForm()
        {
            InitializeComponent();

            _programSettings = new ProgramSettings(ConfigurationManager.AppSettings);
            _observers = new List<IObserver> { new TextBoxOutputObserver(outputTextBox) };

            _utilityProvider = new DefaultUtilityProvider();
            
            _processors = new Dictionary<string, IProcessor>
            {
                {".wav", new AudioProcessor(_programSettings, _utilityProvider, _observers)},
            };
        }

        private void FormLoadHandler(object sender, EventArgs e)
        {
            try
            {
                WriteHelloMessage();
                _programSettings.Verify();

                // want to get all files in the input directory
                // and convert them to file models
                // and then take all of the files that are valid
                // and start with the correct project code
                // and then group them by bar code
                var batchGroups = Directory.EnumerateFiles(_programSettings.InputDirectory)
                    .Select(p => new FileModel(p))
                    .Where(f => f.IsValidForGrouping())
                    .Where(f => f.BelongsToProject(_programSettings.ProjectCode))
                    .GroupBy(f => f.BarCode).ToList();
                   
                // to do: catch exception and prompt user to retry, ignore, or cancel
                // if retry move group files back to input and start over
                foreach (var group in batchGroups)
                {
                    var processor = GetProcessor(group);
                    processor.ProcessFile(group);
                }

                WriteGoodbyeMessage();
            }
            catch (Exception ex)
            {
                _observers.Log("Fatal Exception Occurred: {0}", ex);
            }
        }

        
        private IProcessor GetProcessor(IEnumerable<FileModel> group )
        {
            // for each model in the group
            // take those that have extensions associated with a processor
            // and group them by that extension
            var validExtensions = group
                .Where(m => _processors.Keys.Contains(m.Extension))
                .GroupBy(m => m.Extension).ToList();
            
            // if we have no groups or if we have more than one group, we have a problem
            if (validExtensions.Count() != 1)
            {
                throw new Exception("Can not determine extension for file batch");
            }

            // get processor for the group's common extension
            return _processors[validExtensions.First().Key];
        }
        
        private void WriteHelloMessage()
        {
            _observers.Log("Starting {0}", DateTime.Now);
        }

        private void WriteGoodbyeMessage()
        {
            _observers.Log("Completed {0}", DateTime.Now);
        }

        
    }
}