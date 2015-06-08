using System.Collections.Generic;
using System.IO;
using System.Linq;
using Packager.Models.FileModels;
using Packager.Models.PodMetadataModels;
using Packager.Providers;

namespace Packager.Models.BextModels
{
    public class ConformancePointDocumentFactory
    {
        private IFileProvider Provider { get; set; }
        private string ProcessingDirectory { get; set; }
        private string DigitizingEntity { get; set; }
        private string Institution { get; set; }

        public ConformancePointDocumentFactory(IFileProvider fileProvider, string processingDirectory, string digitizingEntity, string institution)
        {
            Provider = fileProvider;
            ProcessingDirectory = processingDirectory;
            DigitizingEntity = digitizingEntity;
            Institution = institution;
        }

        public ConformancePointDocument Get(IEnumerable<ObjectFileModel> filesToAddMetadata, PodMetadata metadata)
        {
            var result = new ConformancePointDocument
            {
                File = (from fileModel in filesToAddMetadata
                    let fullPath = Path.Combine(ProcessingDirectory, fileModel.ToFileName())
                    let fileInfo = Provider.GetFileInfo(fullPath)
                    select new ConformancePointDocumentFile
                    {
                        Name = fullPath,
                        Core = new ConformancePointDocumentFileCore
                        {
                            Originator = DigitizingEntity,
                            OriginatorReference = Path.GetFileNameWithoutExtension(fileModel.ToFileName()),
                            Description = GetBextDescription(metadata, fileModel),
                            ICMT = GetBextDescription(metadata, fileModel),
                            IARL = string.Format("{0}.", Institution),
                            OriginationDate = fileInfo.CreationTime.ToString("yyyy:MM:dd"),
                            OriginationTime = fileInfo.CreationTime.ToString("HH:mm:ss"),
                            TimeReference = "0",
                            ICRD = fileInfo.CreationTime.ToString("yyyy:MM:dd"),
                            INAM = metadata.Data.Object.Details.Title
                        }
                    }).ToArray()
            };

            return result;
            
        }

        private string GetBextDescription(PodMetadata metadata, ObjectFileModel fileModel)
        {
            return string.Format("{0}. {1}. File use: {2}. {3}",
                Institution,
                metadata.Data.Object.Details.CallNumber,
                fileModel.FullFileUse, fileModel.ToFileName());
        }
    }
}