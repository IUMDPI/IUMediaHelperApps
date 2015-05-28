using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;
using Packager.Extensions;
using Packager.Models;
using Packager.Observers;

namespace Packager.Utilities
{
    public class AudioProcessor : AbstractProcessor
    {
        public AudioProcessor(IProgramSettings programSettings, List<IObserver> observers) : base(programSettings, observers)
        {
        }

        public override void ProcessFile(string fileName)
        {
            Observers.LogHeader("Embedding Metadata: {0}", fileName);

            // move the file to our work dir
            var targetPath = MoveFileToProcessing(fileName);

            const string standinDescription =
                "Indiana University, Bloomington. William and Gayle Cook Music Library. TP-S .A1828 81-4-17 v. 1. File use:";

            const string standinIARL =
                "Indiana University, Bloomington. William and Gayle Cook Music Library. TP-S .A1828 81-4-17 v. 1. File use:";

            const string standinICMT = "Indiana University, Bloomington. William and Gayle Cook Music Library.";

            var data = new BextData
            {
                Description = standinDescription,
                IARL = standinIARL,
                ICMT = standinICMT
            };

            AddMetadata(targetPath, data);

            Observers.LogHeader("Generating Mezzanine Version: {0}", fileName);
            var mezzanineFilePath = CreateMezzanine(targetPath, FFMPEGAudioMezzanineArguments);

            Observers.LogHeader("Generating AccessVersion: {0}", fileName);
            CreateAccess(mezzanineFilePath, FFMPEGAudioAccessArguments);

            //todo: figure out how to get canonical name

            Observers.LogHeader("Generating Xml: {0}", fileName);

            var carrierData = GenerateCarrierData();
            var xmlDataString = new XmlExporter().GenerateXml(new IU {Carrier  = carrierData});
            var foo = "bar";
        }

        private string CreateMezzanine(string inputPath, string commandLineArgs)
        {
            
            var outputPath = Path.Combine(ProcessingDirectory, Path.GetFileNameWithoutExtension(inputPath) + ".aac");
            var args = string.Format("-i {0} {1} {2}", inputPath, commandLineArgs, outputPath);

            CreateDerivative(args);
            return outputPath;
        }

        private void CreateAccess(string inputPath, string commandLineArgs)
        {
            var outputPath = Path.Combine(ProcessingDirectory, Path.GetFileNameWithoutExtension(inputPath) + ".mp4");
            var args = string.Format("-i {0} {1} {2}", inputPath, commandLineArgs, outputPath);

            CreateDerivative(args);
        }

        private void CreateDerivative(string args)
        {
            var startInfo = new ProcessStartInfo(FFMPEGPath)
            {
                Arguments = args,
                RedirectStandardError = true,
                RedirectStandardOutput = true,
                UseShellExecute = false
            };

            var process = Process.Start(startInfo);
            if (process == null)
            {
                throw new Exception(string.Format("Could not start {0}", FFMPEGPath));
            }

            do
            {
                Application.DoEvents();
                process.WaitForExit(5);
            } while (!process.HasExited);

            string output;
            using (var reader = process.StandardError)
            {
                output = reader.ReadToEnd();
            }

            Observers.Log(output);

            if (process.ExitCode != 0)
            {
                throw new Exception(string.Format("Could not generate derivative: {0}", process.ExitCode));
            }
        }

        private void AddMetadata(string targetPath, BextData data)
        {
            var args = string.Format("--verbose --append {0} {1}", string.Join(" ", data.GenerateCommandArgs()), targetPath);

            var startInfo = new ProcessStartInfo(BWFMetaEditPath)
            {
                Arguments = args,
                RedirectStandardError = true,
                RedirectStandardOutput = true,
                UseShellExecute = false
            };

            var process = Process.Start(startInfo);
            if (process == null)
            {
                throw new Exception(string.Format("Could not start {0}", BWFMetaEditPath));
            }

            do
            {
                Application.DoEvents();
                process.WaitForExit(5);
            } while (!process.HasExited);


            string output;
            using (var reader = process.StandardOutput)
            {
                output = reader.ReadToEnd();
            }
            Observers.Log(output);

            if (process.ExitCode != 0 || !SuccessMessagePresent(targetPath, output))
            {
                throw new Exception(string.Format("Could not insert BEXT Data: {0}", process.ExitCode));
            }
        }

        private CarrierData GenerateCarrierData()
        {
            var temporaryInputDataName = MoveFileToProcessing("barcode.xlsx");
            return CarrierData.ImportFromExcel(temporaryInputDataName);
        }

        private static bool SuccessMessagePresent(string fileName, string output)
        {
            var success = string.Format("{0}: is modified", fileName).ToLowerInvariant();
            var nothingToDo = string.Format("{0}: nothing to do", fileName).ToLowerInvariant();
            return output.ToLowerInvariant().Contains(success) || output.ToLowerInvariant().Contains(nothingToDo);
        }
    }
}