using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security.Principal;
using System.Windows.Forms;
using Excel;
using Packager.Extensions;
using Packager.Models;
using Packager.Models.FileModels;
using Packager.Observers;
using Packager.Providers;
using Packager.Utilities;

namespace Packager.Processors
{
    public class AudioProcessor : AbstractProcessor
    {
        public AudioProcessor(IProgramSettings programSettings, IDependencyProvider dependencyProvider, List<IObserver> observers)
            : base(programSettings, dependencyProvider, observers)
        {
        }

        public override void ProcessFile(IGrouping<string, AbstractFileModel> batchGrouping)
        {
            Barcode = batchGrouping.Key;

            // make directory to hold processed files
            DirectoryProvider.CreateDirectory(Path.Combine(ProcessingDirectory));

            Observers.LogHeader("Processing batch {0}", Barcode);

            var excelSpreadSheet = GetExcelSpreadSheet(batchGrouping);

            Observers.Log("Spreadsheet: {0}", excelSpreadSheet.ToFileName());
            
            var filesToProcess = batchGrouping
                .Where(m => m.HasExtension(PreservationFileExtension))
                .Where(m => m.IsObjectModel())
                .Select(m => (ObjectFileModel)m).ToList();

            foreach (var fileModel in filesToProcess)
            {
                Observers.Log("File: {0}", fileModel.OriginalFileName);
            }

            if (filesToProcess.Any(f => f.IsValid() == false))
            {
                throw new Exception("Could not process batch: one or more files has an unexpected filename");
            }

            // go through and process the various files
            // and add them to a list of files that have
            // been processed
            var processedList = new List<AbstractFileModel>();
            processedList = filesToProcess
                .Aggregate(processedList, (current, fileModel) => current.Concat(ProcessFile(fileModel))
                .ToList());

            // using the list of files that have been processed
            // make the xml file
            var xmlModel = GenerateXml(excelSpreadSheet,
                processedList.Where(m => m.IsObjectModel()).Select(m => (ObjectFileModel)m).ToList());

            processedList.Add(xmlModel);

            // make directory to hold completed files
            DirectoryProvider.CreateDirectory(DropBoxDirectory);

            // copy files
            foreach (var fileName in processedList.Select(fileModel => fileModel.ToFileName()))
            {
                Observers.Log("copying {0} to {1}", fileName, DropBoxDirectory);
                File.Copy(
                    Path.Combine(ProcessingDirectory, fileName),
                    Path.Combine(DropBoxDirectory, fileName));
            }

            // done - log new line
            Observers.Log("");

        }

        protected override string ProductionFileExtension
        {
            get { return ".wav"; }
        }

        protected override string AccessFileExtension
        {
            get { return ".mp4"; }
        }

        protected override string MezzanineFileExtension
        {
            get { return ".aac"; }
        }

        protected override string PreservationFileExtension
        {
            get { return ".wav"; }
        }

        private static ExcelFileModel GetExcelSpreadSheet(IEnumerable<AbstractFileModel> batchGrouping)
        {
            var result = batchGrouping.SingleOrDefault(m => m.IsExcelModel());
            if (result == null)
            {
                throw new Exception("No input data spreadsheet in batch");
            }

            return result as ExcelFileModel;
        }

        public override List<ObjectFileModel> ProcessFile(ObjectFileModel objectFileModelModel)
        {
            var fileName = objectFileModelModel.ToFileName();

            Observers.LogHeader("Embedding Metadata: {0}", fileName);

            // move the file to our work dir
            var targetPath = MoveFileToProcessing(objectFileModelModel.ToFileName());

            var data = BextDataProvider.GetMetadata(objectFileModelModel.BarCode);

            AddMetadata(targetPath, data);

            Observers.LogHeader("Generating Mezzanine Version: {0}", fileName);
            var mezzanineModel = CreateDerivative(objectFileModelModel, ToMezzanineFileModel(objectFileModelModel), FFMPEGAudioMezzanineArguments);

            Observers.LogHeader("Generating AccessVersion: {0}", fileName);
            var accessModel = CreateDerivative(mezzanineModel, ToAccessFileModel(mezzanineModel), FFMPEGAudioAccessArguments);

            // return models for files
            return new List<ObjectFileModel> { objectFileModelModel, mezzanineModel, accessModel };
        }

        private ObjectFileModel CreateDerivative(ObjectFileModel originalModel, ObjectFileModel newModel, string commandLineArgs)
        {
            var inputPath = Path.Combine(ProcessingDirectory, originalModel.ToFileName());
            var outputPath = Path.Combine(ProcessingDirectory, newModel.ToFileName());

            var args = string.Format("-i {0} {1} {2}", inputPath, commandLineArgs, outputPath);

            CreateDerivative(args);
            return newModel;
        }

        private void CreateDerivative(string args)
        {
            var startInfo = new ProcessStartInfo(FFMPEGPath)
            {
                Arguments = args,
                RedirectStandardError = true,
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true
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

        private FileData GetFileData(ObjectFileModel objectFileModel)
        {
            var hasher = new Hasher();
            var filePath = Path.Combine(ProcessingDirectory, objectFileModel.ToFileName());
            using (var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
            {
                return new FileData
                {
                    Checksum = hasher.Hash(stream),
                    FileName = Path.GetFileName(filePath)
                };
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
                UseShellExecute = false,
                CreateNoWindow = true
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

        private XmlFileModel GenerateXml(ExcelFileModel excelModel, List<ObjectFileModel> filesToProcess)
        {
            var wrapper = new IU { Carrier = GenerateCarrierDataModel(excelModel, filesToProcess) };
            var xml = XmlExporter.GenerateXml(wrapper);

            var result = new XmlFileModel { BarCode = Barcode, ProjectCode = ProjectCode, Extension = ".xml" };
            SaveXmlFile(string.Format(result.ToFileName(), ProjectCode, Barcode), xml);
            return result;
        }

        private CarrierData GenerateCarrierDataModel(ExcelFileModel excelModel, List<ObjectFileModel> filesToProcess)
        {
            MoveFileToProcessing(excelModel.ToFileName());
            IExcelDataReader excelReader = null;
            try
            {
                var targetPath = Path.Combine(ProcessingDirectory, excelModel.ToFileName());
                using (var stream = new FileStream(targetPath, FileMode.Open, FileAccess.Read))
                {
                    excelReader = ExcelReaderFactory.CreateOpenXmlReader(stream);

                    excelReader.IsFirstRowAsColumnNames = true;
                    var dataSet = excelReader.AsDataSet();

                    var row = dataSet.Tables["InputData"].Rows[0];

                    var result = (CarrierData)ExcelImporter.Import(row);
                    result.Parts.Sides = GenerateSideData(filesToProcess, row);

                    return result;
                }
            }
            finally
            {
                if (excelReader != null)
                {
                    excelReader.Close();
                }
            }
        }

        private SideData[] GenerateSideData(IEnumerable<ObjectFileModel> filesToProcess, DataRow row)
        {
            var sideGroupings = filesToProcess.GroupBy(f => f.SequenceIndicator.ToInteger()).OrderBy(g => g.Key).ToList();
            if (!sideGroupings.Any())
            {
                throw new Exception("Could not determine side groupings");
            }

            var result = new List<SideData>();
            foreach (var grouping in sideGroupings)
            {
                if (!grouping.Key.HasValue)
                {
                    throw new Exception("One or more groupings has an invalid sequence value");
                }

                var sideData = ImportSideData(row, grouping.Key.Value);
                AddIngestMetadata(sideData, grouping.SingleOrDefault(g => g.IsPreservationVersion()));
                sideData.Files = grouping.Select(GetFileData).ToList();

                result.Add(sideData);
            }

            return result.ToArray();
        }

        private void AddIngestMetadata(SideData sideData, ObjectFileModel preservationObjectFileModel)
        {
            var targetPath = Path.Combine(ProcessingDirectory, preservationObjectFileModel.ToFileName());
            var info = new FileInfo(targetPath);
            info.Refresh();

            sideData.Ingest.Date = info.CreationTimeUtc.ToString(DataFormat, CultureInfo.InvariantCulture);

            var owner = info.GetAccessControl().GetOwner(typeof(NTAccount)).Value;
            var userInfo = UserInfoResolver.Resolve(owner);

            sideData.Ingest.CreatedBy = userInfo.DisplayName;

            sideData.Ingest.ExtractionWorkstation = Environment.MachineName;

        }

        private static bool SuccessMessagePresent(string fileName, string output)
        {
            var success = string.Format("{0}: is modified", fileName).ToLowerInvariant();
            var nothingToDo = string.Format("{0}: nothing to do", fileName).ToLowerInvariant();
            return output.ToLowerInvariant().Contains(success) || output.ToLowerInvariant().Contains(nothingToDo);
        }

        private void SaveXmlFile(string filename, string xml)
        {
            File.WriteAllText(Path.Combine(ProcessingDirectory, filename), xml);
        }

        private static SideData ImportSideData(DataRow row, int sideValue)
        {
            return new SideData
            {
                Side = sideValue.ToString(CultureInfo.InvariantCulture),
                Files = new List<FileData>(),
                ManualCheck = row[string.Format("Part-Side-{0}-ManualCheck", sideValue)].ToString(),
                Ingest = new IngestData
                {
                    SpeedUsed = row[string.Format("Part-Side-{0}-Speed_used", sideValue)].ToString(),
                    Comments = row[string.Format("Part-Side-{0}-Comments", sideValue)].ToString()
                }
            };
        }

    }
}