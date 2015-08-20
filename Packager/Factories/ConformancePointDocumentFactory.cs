using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using Packager.Exceptions;
using Packager.Models.BextModels;
using Packager.Models.FileModels;
using Packager.Models.PodMetadataModels;

namespace Packager.Factories
{
    public class ConformancePointDocumentFactory : IConformancePointDocumentFactory
    {
        private string BaseProcessingDirectory { get; set; }
        
        private readonly List<string> _knownDigitalFormats = new List<string>{"cd-r", "dat"};

        public ConformancePointDocumentFactory(string baseProcessingDirectory)
        {
            BaseProcessingDirectory = baseProcessingDirectory;
        }

        public ConformancePointDocumentFile Generate(ObjectFileModel model, DigitalFileProvenance provenance, ConsolidatedPodMetadata metadata)
        {
            var digitizedOn = provenance.DateDigitized.Value;
            var description = GenerateBextDescription(metadata, model);

            return new ConformancePointDocumentFile
            {
                Name = Path.Combine(BaseProcessingDirectory, model.GetFolderName(), model.ToFileName()),
                Core = new ConformancePointDocumentFileCore
                {
                    Originator = metadata.DigitizingEntity,
                    OriginatorReference = Path.GetFileNameWithoutExtension(model.ToFileName()),
                    Description = description,
                    ICMT = description,
                    IARL = metadata.Unit,
                    OriginationDate = GetDateString(provenance.DateDigitized, "yyyy-MM-dd", ""),
                    OriginationTime = GetDateString(provenance.DateDigitized, "HH:mm:ss", ""),
                    TimeReference = "0",
                    ICRD = GetDateString(provenance.DateDigitized, "yyyy-MM-dd",""),
                    INAM = metadata.Title,
                    CodingHistory = GenerateCodingHistory(metadata, provenance)
                }
            };
        }


        private static string GetDateString(DateTime? date, string format, string defaultValue)
        {
            return date.HasValue == false ? defaultValue : date.Value.ToString(format);
        }

/*
        
         private static DateTime GenerateOriginationDateTime(DigitalFileProvenance provenance)
        {
            DateTime result;
            if (DateTime.TryParse(provenance.DateDigitized, out result) == false)
            {
                throw new AddMetadataException("Could not convert {0} to date time object", provenance.DateDigitized);
            }

            return result;
        }*/

        private static string GenerateBextDescription(ConsolidatedPodMetadata metadata, ObjectFileModel fileModel)
        {
            return string.Format("{0}. {1}. File use: {2}. {3}",
                metadata.Unit,
                metadata.CallNumber,
                fileModel.FullFileUse,
                Path.GetFileNameWithoutExtension(fileModel.ToFileName()));
        }

        private string GetFormatText(ConsolidatedPodMetadata metadata)
        {
            return _knownDigitalFormats.Contains(metadata.Format.ToLowerInvariant()) 
                ? "DIGITAL" 
                : "ANALOG";
        }

        private const string CodingHistoryLine1Format = "A={0},M={1},T={2},\r\n";
        private const string CodingHistoryLine2Format = "A=PCM,F=96000,W=24,M={0},T={1};A/D,\r\n";
        private const string CodingHistoryLine3 = "A=PCM,F=96000,W=24,M=mono,T=Lynx AES16;DIO";

        private static string GenerateLine1TextField(ConsolidatedPodMetadata metadata, DigitalFileProvenance provenance)
        {
            var parts = new List<string>
            {
                string.Format("{0} {1}", provenance.PlayerManufacturer, provenance.PlayerModel),
                provenance.PlayerSerialNumber,
                DetermineSpeedUsed(metadata, provenance),
                metadata.Format
            };

            return string.Join(";", parts.Where(p => !string.IsNullOrWhiteSpace(p)));
        }

        private static string GenerateLine2TextField(DigitalFileProvenance provenance)
        {
            var parts = new List<string>
            {
                string.Format("{0} {1}", provenance.AdManufacturer, provenance.AdModel),
                provenance.AdSerialNumber
            };

            return string.Join(";", parts.Where(p => !string.IsNullOrWhiteSpace(p)));
        }

        private string GenerateCodingHistory(ConsolidatedPodMetadata metadata, DigitalFileProvenance provenance)
        {
            var builder = new StringBuilder();

            builder.AppendFormat(CodingHistoryLine1Format,
                GetFormatText(metadata),
                metadata.SoundField,
                GenerateLine1TextField(metadata, provenance));

            builder.AppendFormat(CodingHistoryLine2Format,
                metadata.SoundField,
                GenerateLine2TextField(provenance));

            builder.Append(CodingHistoryLine3);

            return builder.ToString();
        }

        // if provenance.speedUsed is present
        // otherwise use metadata.playback speed
        // finally, replace comma delimiters with semi-colons and remove spaces
        private static string DetermineSpeedUsed(ConsolidatedPodMetadata metadata, DigitalFileProvenance provenance)
        {
            var result = string.IsNullOrWhiteSpace(provenance.SpeedUsed)
                ? metadata.PlaybackSpeed
                : provenance.SpeedUsed;

            if (string.IsNullOrWhiteSpace(result))
            {
                return result;
            }

            return result.Replace(",", ";").Replace(" ","");
        }
    }
}