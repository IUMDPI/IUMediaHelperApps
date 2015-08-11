using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Packager.Exceptions;
using Packager.Models.BextModels;
using Packager.Models.FileModels;
using Packager.Models.PodMetadataModels;

namespace Packager.Factories
{
    public class ConformancePointDocumentFactory : IConformancePointDocumentFactory
    {
        private const string CodingHistoryLine1Format = "A={0},M={1},T={2} {3};{4};{5};{6},\r\n";
        private const string CodingHistoryLine2Format = "A=PCM,F=96000,W=24,M={0},T={1} {2};{3};A/D,\r\n";
        private const string CodingHistoryLine3 = "A=PCM,F=96000,W=24,M=mono,T=Lynx AES16;DIO";
        
        private readonly List<string> _knownDigitalFormats = new List<string>{"cd-r", "dat"}; 
        
        public ConformancePointDocumentFile Generate(ObjectFileModel model, DigitalFileProvenance provenance, ConsolidatedPodMetadata metadata, string processingDirectory)
        {
            var digitizedOn = GenerateOriginationDateTime(provenance);
            var description = GenerateBextDescription(metadata, model);

            return new ConformancePointDocumentFile
            {
                Name = Path.Combine(processingDirectory, model.ToFileName()),
                Core = new ConformancePointDocumentFileCore
                {
                    Originator = metadata.DigitizingEntity,
                    OriginatorReference = Path.GetFileNameWithoutExtension(model.ToFileName()),
                    Description = description,
                    ICMT = description,
                    IARL = metadata.Unit,
                    OriginationDate = digitizedOn.ToString("yyyy-MM-dd"),
                    OriginationTime = digitizedOn.ToString("HH:mm:ss"),
                    TimeReference = "0",
                    ICRD = digitizedOn.ToString("yyyy-MM-dd"),
                    INAM = metadata.Title,
                    CodingHistory = GenerateCodingHistory(metadata, provenance)
                }
            };
        }

        private static DateTime GenerateOriginationDateTime(DigitalFileProvenance provenance)
        {
            DateTime result;
            if (DateTime.TryParse(provenance.DateDigitized, out result) == false)
            {
                throw new AddMetadataException("Could not convert {0} to date time object", provenance.DateDigitized);
            }

            return result;
        }

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

        private string GenerateCodingHistory(ConsolidatedPodMetadata metadata, DigitalFileProvenance provenance)
        {
            var builder = new StringBuilder();

            builder.AppendFormat(CodingHistoryLine1Format,
                GetFormatText(metadata),
                metadata.SoundField,
                provenance.PlayerManufacturer,
                provenance.PlayerModel,
                provenance.PlayerSerialNumber,
                metadata.PlaybackSpeed,
                metadata.Format);

            builder.AppendFormat(CodingHistoryLine2Format,
                metadata.SoundField,
                provenance.AdManufacturer,
                provenance.AdModel,
                provenance.AdSerialNumber);

            builder.Append(CodingHistoryLine3);

            return builder.ToString();
        }
    }
}