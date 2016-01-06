using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Controls;

namespace Recorder.Models
{
    public class ObjectModel
    {
        public ObjectModel(ProgramSettings settings)
        {
            Settings = settings;

            FileUses = new List<Tuple<string, string>>
            {
                new Tuple<string, string>("Preservation", "pres"),
                new Tuple<string, string>("Preservation-Intermediate", "pres-int"),
                new Tuple<string, string>("Mezzanine", "mezz")
            };

            PossibleChannels = new List<Tuple<string, int>>
            {
                new Tuple<string, int>("2 channels", 2),
                new Tuple<string, int>("4 channels", 4)
            };

            Channels = 2;
        }
        
        public List<Tuple<string, string>> FileUses { get; private set; }

        private readonly Regex _barcodeExpression = new Regex(@"^\d{14,14}$");
        protected ProgramSettings Settings { get; }
        public string Barcode { get; set; }
        public int Part { get; set; }
        public string ProjectCode => Settings.ProjectCode;
        public string FileUse { get; set; }

        public int Channels { get; set; }

        public string Filename => FilePartsValid().IsValid
            ? string.Format($"{ProjectCode}_{Barcode}_{Part:d2}_{FileUse}.mkv")
            : string.Empty;

        public string WorkingFolderName => FilePartsValid().IsValid ? $"{ProjectCode}_{Barcode}":"";
        public string WorkingFolderPath => Path.Combine(Settings.WorkingFolder, WorkingFolderName);

        public string OutputFolder => Path.Combine(Settings.OutputFolder, WorkingFolderName);
        public string OutputFile => Path.Combine(OutputFolder, Filename);

        public string ExistingPartsMask => $"{ProjectCode}_{Barcode}_part_*.mkv";
        public List<Tuple<string, int>> PossibleChannels { get; set; }

        public ValidationResult FilePartsValid()
        {
            if (BarcodeValid() == false)
            {
                return new ValidationResult(false, "invalid barcode");
            }

            if (PartValid() == false)
            {
                return new ValidationResult(false, "invalid part");
            }

            if (FileUseValid() == false)
            {
                return new ValidationResult(false, "invaid file use");
            }

            return new ValidationResult(true, "");
        }

        private bool BarcodeValid()
        {
            return !string.IsNullOrWhiteSpace(Barcode) && _barcodeExpression.IsMatch(Barcode);
        }

        private bool PartValid()
        {
            return Part >= 1 && Part <= 5;
        }

        private bool FileUseValid()
        {
            return string.IsNullOrWhiteSpace(FileUse) == false;
        }

        public bool PartsPresent()
        {
            if (!BarcodeValid())
            {
                return false;
            }

            return Directory.Exists(WorkingFolderPath)
                   && Directory.EnumerateFiles(WorkingFolderPath, ExistingPartsMask).Any();
        }

        public bool MultiplePartsPresent()
        {
            if (!BarcodeValid())
            {
                return false;
            }

            return Directory.Exists(WorkingFolderPath)
                   && Directory.EnumerateFiles(WorkingFolderPath, ExistingPartsMask).Count()>1;
        }
    }
}