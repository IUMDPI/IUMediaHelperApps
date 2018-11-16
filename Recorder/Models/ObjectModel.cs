using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Controls;
using Common.Models;

namespace Recorder.Models
{
    public class ObjectModel
    {
        public ObjectModel(ProgramSettings settings)
        {
            Settings = settings;

            FileUsages = new List<IFileUsage>
            {
                Common.Models.FileUsages.PreservationMaster,
                Common.Models.FileUsages.PreservationIntermediateMaster,
                Common.Models.FileUsages.MezzanineFile
            };

            PossibleChannelsAndStreams = new List<AudioChannelsAndStreams>
            {
                new AudioChannelsAndStreams
                {
                    Channels = 1, Streams = 1, Id = 1, DisplayName = "1 audio channel (1 mono stream)"
                },
                new AudioChannelsAndStreams
                {
                    Channels = 2, Streams = 1, Id = 2, DisplayName = "2 audio channels (1 stereo stream)"
                },
                new AudioChannelsAndStreams
                {
                    Channels = 2, Streams = 2, Id = 3, DisplayName = "2 audio channels (2 mono streams)"
                },
                new AudioChannelsAndStreams
                {
                    Channels = 4, Streams = 4, Id = 4, DisplayName = "4 audio channels (4 mono streams)"
                }
            };
            
            SelectedChannelsAndStreams = PossibleChannelsAndStreams.Count < 2 
                ? PossibleChannelsAndStreams.FirstOrDefault()
                : PossibleChannelsAndStreams[1];
        }
        
        public List<IFileUsage> FileUsages { get; private set; }

        private readonly Regex _barcodeExpression = new Regex(@"^\d{14,14}$");
        protected ProgramSettings Settings { get; }
        public string Barcode { get; set; }
        public int Part { get; set; }
        public string ProjectCode => Settings.ProjectCode;
        public IFileUsage FileUse { get; set; }

        public AudioChannelsAndStreams SelectedChannelsAndStreams { get; set; }

        public string Filename => FilePartsValid().IsValid
            ? string.Format($"{ProjectCode}_{Barcode}_{Part:d2}_{FileUse.FileUse}.mkv")
            : string.Empty;

        public string WorkingFolderName => FilePartsValid().IsValid ? $"{ProjectCode}_{Barcode}":"";
        public string WorkingFolderPath => Path.Combine(Settings.WorkingFolder, WorkingFolderName);

        public string OutputFolder => Path.Combine(Settings.OutputFolder, WorkingFolderName);
        public string OutputFile => Path.Combine(OutputFolder, Filename);

        public string ExistingPartsMask => $"{ProjectCode}_{Barcode}_part_*.mkv";
        public List<AudioChannelsAndStreams> PossibleChannelsAndStreams { get; }
        
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
            return FileUse !=null;
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