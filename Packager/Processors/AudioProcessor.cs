using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security.Principal;
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

        protected override string PreservationIntermediateFileExtenstion
        {
            get { return ".wav"; }
        }

        public override void ProcessFile(IGrouping<string, AbstractFileModel> barcodeGrouping)
        {
            Barcode = barcodeGrouping.Key;

            // make directory to hold processed files
            DirectoryProvider.CreateDirectory(Path.Combine(ProcessingDirectory));

            Observers.LogHeader("Processing object {0}", Barcode);

            var excelSpreadSheet = GetExcelSpreadSheet(barcodeGrouping);

            Observers.Log("Spreadsheet: {0}", excelSpreadSheet.ToFileName());

            var filesToProcess = barcodeGrouping
                .Where(m => m.IsObjectModel())
                .Select(m => (ObjectFileModel) m)
                .Where(m => m.IsPreservationIntermediateVersion() || m.IsPreservationVersion())
                .ToList();

            // make sure files are ok
            if (filesToProcess.Any(f => f.IsValid() == false))
            {
                throw new Exception("Could not process batch: one or more files has an unexpected filename");
            }

            // now move them to processing
            foreach (var fileModel in filesToProcess)
            {
                Observers.Log("Moving file to processing: {0}", fileModel.OriginalFileName);
                MoveFileToProcessing(fileModel.ToFileName());
            }

            // create derivatives for the various files
            // and add them to a list of files that have
            // been processed
            var processedList = new List<AbstractFileModel>();
            processedList = processedList.Concat(filesToProcess).ToList();

            // first group by sequence
            // then determine which file to use to create derivatives
            // then use that file to create the derivatives
            // then aggregate the results into the processed list
            processedList = filesToProcess.GroupBy(m => m.SequenceIndicator)
                .Select(GetPreservationOrIntermediateModel)
                .Select(CreateDerivatives)
                .Aggregate(processedList, (current, derivatives) => current.Concat(derivatives)
                    .ToList());
            
            // now add metadata to eligible objects
            AddMetadata(processedList);

            // using the list of files that have been processed
            // make the xml file
            var xmlModel = GenerateXml(excelSpreadSheet,
                processedList.Where(m => m.IsObjectModel()).Select(m => (ObjectFileModel) m).ToList());

            processedList.Add(xmlModel);

            // make directory to hold completed files
            DirectoryProvider.CreateDirectory(DropBoxDirectory);

            // copy files
            foreach (var fileName in processedList.Select(fileModel => fileModel.ToFileName()).OrderBy(f=>f))
            {
                Observers.Log("copying {0} to {1}", fileName, DropBoxDirectory);
                File.Copy(
                    Path.Combine(ProcessingDirectory, fileName),
                    Path.Combine(DropBoxDirectory, fileName));
            }

            // done - log new line
            Observers.Log("");
        }

        private static ObjectFileModel GetPreservationOrIntermediateModel<T>(IGrouping<T, ObjectFileModel> grouping)
        {
            var preservationIntermediate = grouping.SingleOrDefault(m => m.IsPreservationIntermediateVersion());
            return preservationIntermediate ?? grouping.SingleOrDefault(m => m.IsPreservationVersion());
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

        public override List<ObjectFileModel> CreateDerivatives(ObjectFileModel fileModel)
        {
            var fileName = fileModel.ToFileName();

            Observers.LogHeader("Generating Production Version: {0}", fileName);
            var prodModel = CreateDerivative(fileModel, ToProductionFileModel(fileModel), FFMPEGAudioProductionArguments);

            Observers.LogHeader("Generating AccessVersion: {0}", fileName);
            var accessModel = CreateDerivative(prodModel, ToAccessFileModel(prodModel), FFMPEGAudioAccessArguments);

            // return models for files
            return new List<ObjectFileModel> {prodModel, accessModel};
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

            var output = process.StandardError.ReadToEnd();
            process.WaitForExit();

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

        private void AddMetadata(IEnumerable<AbstractFileModel> processedList)
        {
            var filesToAddMetadata = processedList.Where(m => m.IsObjectModel())
                .Select(m => (ObjectFileModel) m)
                .Where(m => m.IsAccessVersion() == false).ToList();

            if (!filesToAddMetadata.Any())
            {
                throw new Exception("Could not add metadata: no eligible files");
            }

            var data = BextDataProvider.GetMetadata(Barcode);

            foreach (var fileModel in filesToAddMetadata)
            {
                AddMetadata(fileModel, data);
            }
        }

        private void AddMetadata(ObjectFileModel fileModel, BextData data)
        {
            var targetPath = Path.Combine(ProcessingDirectory, fileModel.ToFileName());
            Observers.LogHeader("Embedding Metadata: {0}", fileModel.ToFileName());

            AddMetadata(targetPath, data);
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

            var output = process.StandardOutput.ReadToEnd();
            process.WaitForExit();

            Observers.Log(output);

            if (process.ExitCode != 0 || !SuccessMessagePresent(targetPath, output))
            {
                throw new Exception(string.Format("Could not insert BEXT Data: {0}", process.ExitCode));
            }
        }

        private XmlFileModel GenerateXml(ExcelFileModel excelModel, List<ObjectFileModel> filesToProcess)
        {
            var wrapper = new IU {Carrier = GenerateCarrierDataModel(excelModel, filesToProcess)};
            var xml = XmlExporter.GenerateXml(wrapper);

            var result = new XmlFileModel {BarCode = Barcode, ProjectCode = ProjectCode, Extension = ".xml"};
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

                    var result = (CarrierData) ExcelImporter.Import(row);
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
                AddIngestMetadata(sideData, GetPreservationOrIntermediateModel(grouping));
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

            var owner = info.GetAccessControl().GetOwner(typeof (NTAccount)).Value;
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