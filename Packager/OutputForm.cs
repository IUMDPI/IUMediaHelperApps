using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
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

        public OutputForm()
        {
            InitializeComponent();

            _programSettings = new ProgramSettings(ConfigurationManager.AppSettings);
            _observers = new List<IObserver> { new TextBoxOutputObserver(outputTextBox) };
 
            _processors = new Dictionary<string, IProcessor>
            {
                {".wav", new AudioProcessor(_programSettings, _observers)},
                {".xlsx", new SkippingProcessor()}
            };
        }

        private void FormLoadHandler(object sender, EventArgs e)
        {
            try
            {
                WriteHelloMessage();
                _programSettings.Verify();

                var filesToProcess = Directory.EnumerateFiles(_programSettings.InputDirectory);
                foreach (var filePath in filesToProcess)
                {
                    IProcessor processor;
                    var extension = Path.GetExtension(filePath).ToLowerInvariant();
                    _processors.TryGetValue(extension, out processor);
                    if (processor == null)
                    {
                        throw new Exception(string.Format("No processor found for extension: {0}", extension));
                    }

                    processor.ProcessFile(Path.GetFileName(filePath));
                    // insert chunks (BEXT, INFO, IARL)


                    // generate MD5 hash


                    // generate derivatives


                    // move files to fileshare setup on server
                }

                WriteGoodbyeMessage();
            }
            catch (Exception ex)
            {
                _observers.Log("Fatal Exception Occurred: {0}", ex.Message);
            }
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