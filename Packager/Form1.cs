using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Windows.Forms;
using Packager.Extensions;
using Packager.Observers;
using Packager.Utilities;

namespace Packager
{
    public partial class Form1 : Form
    {
        private readonly string _bwfMetaEditPath;
        private readonly string _ffmpegPath;
        private readonly string _inputDirectory;
        private readonly string _processingDirectory;
        private readonly Dictionary<string, IProcessor> _processors;
        private readonly string _ffmpegAudioMezzanineArguments;
        private string _ffmpegAudioAccessArguments;

        private List<IObserver> _observers; 

        public Form1()
        {
            InitializeComponent();

            _inputDirectory = ConfigurationSettings.AppSettings["WhereStaffWorkDirectoryName"];
            _processingDirectory = ConfigurationSettings.AppSettings["ProcessingDirectoryName"];
            _ffmpegPath = ConfigurationSettings.AppSettings["PathToFFMpeg"];
            _bwfMetaEditPath = ConfigurationSettings.AppSettings["PathToMetaEdit"];
            _ffmpegAudioMezzanineArguments = ConfigurationSettings.AppSettings["ffmpegAudioMezzanineArguments"];
            _ffmpegAudioAccessArguments = ConfigurationSettings.AppSettings["ffmpegAudioAccessArguments"];

            _processors = new Dictionary<string, IProcessor>
            {
                {".wav", new AudioProcessor(_ffmpegPath, _bwfMetaEditPath, _ffmpegAudioMezzanineArguments, _ffmpegAudioAccessArguments, _inputDirectory, _processingDirectory)},
                {".xlsx", new SkippingProcessor()}
            };

            _observers = new List<IObserver>{new TextBoxOutputObserver(outputTextBox)};
        }

        private void FormLoadHandler(object sender, EventArgs e)
        {
            try
            {
                WriteHelloMessage();
                VerifyPaths();

                var filesToProcess = Directory.EnumerateFiles(_inputDirectory);
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
            }
            catch (Exception ex)
            {
                _observers.Log("Fatal Exception Occurred: {0}", ex.Message);
            }
        }

       

        private void WriteHelloMessage()
        {
            _observers.Log("Starting {0}\n\n", DateTime.Now);
        }

        private void VerifyPaths()
        {
            if (!Directory.Exists(_inputDirectory))
            {
                throw new DirectoryNotFoundException(_inputDirectory);
            }

            if (!Directory.Exists(_processingDirectory))
            {
                throw new DirectoryNotFoundException(_processingDirectory);
            }

            if (!File.Exists(_ffmpegPath))
            {
                throw new FileNotFoundException(_ffmpegPath);
            }

            if (!File.Exists(_bwfMetaEditPath))
            {
                throw new FileNotFoundException(_bwfMetaEditPath);
            }
        }
    }
}