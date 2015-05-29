using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
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

        public override void ProcessFile(IGrouping<string, FileModel> batchGrouping)
        {
            Barcode = batchGrouping.Key;
            Observers.LogHeader("Processing batch {0}", Barcode);

            // make directory to hold processed files
            var batchFolder = Directory.CreateDirectory(Path.Combine(ProcessingDirectory, Barcode));
            
            var excelSpreadSheet = GetExcelSpreadSheet(batchGrouping);
            Observers.Log("Spreadsheet: {0}", excelSpreadSheet.OriginalFileName);

            var filesToProcess = batchGrouping.Where(m => m.Extension.Equals(SupportedExtension, StringComparison.InvariantCultureIgnoreCase)).ToList();
            foreach (var fileModel in filesToProcess)
            {
                Observers.Log("File: {0}", fileModel.OriginalFileName);
            }
            
            if (filesToProcess.Any(f => f.IsValidForProcessing() == false))
            {
                throw new Exception("Could not process batch: one or more files has an unexpected filename");
            }

            foreach (var fileModel in filesToProcess)
            {
                ProcessFile(fileModel);
            }


            //throw new NotImplementedException();
        }

        public override string SupportedExtension
        {
            get { return ".wav"; }
        }


        private FileModel GetExcelSpreadSheet(IGrouping<string, FileModel> batchGrouping)
        {
            var result = batchGrouping.SingleOrDefault(m => m.Extension.Equals(".xlsx", StringComparison.InvariantCultureIgnoreCase));
            if (result == null)
            {
                throw new Exception("No input data spreadsheet in batch");
            }

            return result;
        }
        

        public override void ProcessFile(FileModel fileModel)
        {
            var fileName = fileModel.ToFileName();
            
            Observers.LogHeader("Embedding Metadata: {0}", fileName);

            // move the file to our work dir
            var targetPath = MoveFileToProcessing(fileModel.OriginalFileName);

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
            var mezzanineModel = CreateMezzanine(fileModel, FFMPEGAudioMezzanineArguments);

            Observers.LogHeader("Generating AccessVersion: {0}", fileName);
            CreateAccess(mezzanineModel, FFMPEGAudioAccessArguments);

            //todo: figure out how to get canonical name

            /*Observers.LogHeader("Generating Xml: {0}", fileName);

            var carrierData = GenerateCarrierData();
            carrierData.Parts.Sides[0].Files = GetFileHashes(fileName);

            var xmlDataString = new XmlExporter().GenerateXml(new IU {Carrier  = carrierData});
            SaveXmlFile(string.Format("{0}.xml", Path.GetFileNameWithoutExtension(fileName)), xmlDataString);*/
        }

        

        private FileModel CreateMezzanine(FileModel originalModel, string commandLineArgs)
        {
            var inputPath = Path.Combine(ProcessingDirectory, Barcode, originalModel.ToFileName());

            var mezzanineModel = new FileModel(originalModel, "mezz", ".aac");
            var outputPath = Path.Combine(ProcessingDirectory, Barcode, mezzanineModel.ToFileName());
            
            var args = string.Format("-i {0} {1} {2}", inputPath, commandLineArgs, outputPath);

            CreateDerivative(args);
            return mezzanineModel;
        }

        private FileModel CreateAccess(FileModel originalModel, string commandLineArgs)
        {
            var inputPath = Path.Combine(ProcessingDirectory, Barcode, originalModel.ToFileName());

            var accessModel = new FileModel(originalModel, "access", ".mp4");
            var outputPath = Path.Combine(ProcessingDirectory, Barcode, accessModel.ToFileName());

            var args = string.Format("-i {0} {1} {2}", inputPath, commandLineArgs, outputPath);

            CreateDerivative(args);
            return accessModel;
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

        private List<FileData> GetFileHashes(string fileName)
        {
            var result = new List<FileData>();
            var searchPattern = string.Format("{0}.*", Path.GetFileNameWithoutExtension(fileName));
            var hasher = new Hasher();
            foreach (var filePath in Directory.GetFiles(ProcessingDirectory, searchPattern))
            {
                using (var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
                {
                    result.Add(new FileData()
                    {
                        Checksum = hasher.Hash(stream),
                        FileName = Path.GetFileName(filePath)
                    });
                }
            }

            return result;
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

        private void SaveXmlFile(string filename, string xml)
        {
            File.WriteAllText(Path.Combine(ProcessingDirectory, filename),xml);
        }
        
    }
}