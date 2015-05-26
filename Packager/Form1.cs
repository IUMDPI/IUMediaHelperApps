using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Windows.Forms;
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

        public Form1()
        {
            InitializeComponent();

            _inputDirectory = ConfigurationSettings.AppSettings["WhereStaffWorkDirectoryName"];
            _processingDirectory = ConfigurationSettings.AppSettings["ProcessingDirectoryName"];
            _ffmpegPath = ConfigurationSettings.AppSettings["PathToFFMpeg"];
            _bwfMetaEditPath = ConfigurationSettings.AppSettings["PathToMetaEdit"];

            _processors = new Dictionary<string, IProcessor>
            {
                {".wav", new AudioProcessor(_ffmpegPath, _bwfMetaEditPath)}
            };
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
                    // move the file to our work dir
                    var targetPath = MoveFileToProcessing(filePath);

                    IProcessor processor;
                    var extension = Path.GetExtension(targetPath).ToLowerInvariant();
                    _processors.TryGetValue(extension, out processor);
                    if (processor == null)
                    {
                        throw new Exception(string.Format("No processor found for extension: {0}", extension));
                    }

                    processor.ProcessFile(targetPath);
                    // insert chunks (BEXT, INFO, IARL)


                    // generate MD5 hash


                    // generate derivatives


                    // move files to fileshare setup on server
                }
            }
            catch (Exception ex)
            {
                outputTextBox.AppendText(string.Format("Fatal Exception Occurred: {0}", ex.Message));
            }
        }

        private string MoveFileToProcessing(string sourcePath)
        {
            if (string.IsNullOrWhiteSpace(Path.GetFileName(sourcePath)))
            {
                throw new Exception(string.Format("Could not parse file: {0}", sourcePath));
            }

            var targetPath = Path.Combine(_processingDirectory, Path.GetFileName(sourcePath));
            File.Move(sourcePath, targetPath);
            return targetPath;
        }

        private void WriteHelloMessage()
        {
            outputTextBox.AppendText(string.Format("Starting {0}\n\n", DateTime.Now));
        }

        private void AppendToOutput(string text)
        {
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
                //    throw new FileNotFoundException(_ffmpegPath);
            }

            if (!File.Exists(_bwfMetaEditPath))
            {
                throw new FileNotFoundException(_bwfMetaEditPath);
            }
        }
    }
}