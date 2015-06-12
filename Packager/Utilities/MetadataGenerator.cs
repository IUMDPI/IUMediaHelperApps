using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security.Principal;
using Packager.Extensions;
using Packager.Models;
using Packager.Models.FileModels;
using Packager.Models.OutputModels;
using Packager.Models.PodMetadataModels;
using Packager.Providers;

namespace Packager.Utilities
{
    public class MetadataGenerator : IMetadataGenerator
    {
        public MetadataGenerator(IFileProvider fileProvider, IHasher hasher, IProgramSettings programSettings, IUserInfoResolver userInfoResolver)
        {
            FileProvider = fileProvider;
            Hasher = hasher;
            ProgramSettings = programSettings;
            UserInfoResolver = userInfoResolver;
        }

        private IUserInfoResolver UserInfoResolver { get; set; }
        private IFileProvider FileProvider { get; set; }
        private IProgramSettings ProgramSettings { get; set; }

        private string DateFormat
        {
            get { return ProgramSettings.DateFormat; }
        }

        private IHasher Hasher { get; set; }

        public CarrierData GenerateMetadata(ConsolidatedPodMetadata podMetadata, IEnumerable<ObjectFileModel> filesToProcess, string processingDirectory)
        {
            var result = CarrierData.FromPodMetadata(podMetadata);
            result.Parts = new PartsData
            {
                DigitizingEntity = ProgramSettings.DigitizingEntity,
                Sides = GenerateSideData(filesToProcess, processingDirectory)
            };

            return result;
        }
        
        private SideData[] GenerateSideData(IEnumerable<ObjectFileModel> filesToProcess, string processingDirectory)
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

                var sideData = ImportSideData(grouping.Key.Value);
                AddIngestMetadata(sideData, grouping.GetPreservationOrIntermediateModel(), processingDirectory);
                sideData.Files = grouping.Select(m => GetFileData(m, processingDirectory)).ToList();

                result.Add(sideData);
            }

            return result.ToArray();
        }

        private void AddIngestMetadata(SideData sideData, ObjectFileModel preservationObjectFileModel, string processingDirectory)
        {
            var targetPath = Path.Combine(processingDirectory, preservationObjectFileModel.ToFileName());
            var info = FileProvider.GetFileInfo(targetPath);
            info.Refresh();

            sideData.Ingest.Date = info.CreationTime.ToString(DateFormat, CultureInfo.InvariantCulture);

            var owner = info.GetAccessControl().GetOwner(typeof (NTAccount)).Value;
            var userInfo = UserInfoResolver.Resolve(owner);

            sideData.Ingest.CreatedBy = userInfo.DisplayName;

            sideData.Ingest.ExtractionWorkstation = Environment.MachineName;
        }

        private static SideData ImportSideData(int sideValue)
        {
            return new SideData
            {
                Side = sideValue.ToString(CultureInfo.InvariantCulture),
                Files = new List<FileData>(),
                ManualCheck = "Unknown",
                Ingest = new IngestData
                {
                    SpeedUsed = "Unknown",
                    Comments = "Unknown"
                }
            };
        }

        private FileData GetFileData(AbstractFileModel objectFileModel, string processingDirectory)
        {
            var filePath = Path.Combine(processingDirectory, objectFileModel.ToFileName());
            using (var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
            {
                return new FileData
                {
                    Checksum = Hasher.Hash(stream),
                    FileName = Path.GetFileName(filePath)
                };
            }
        }
    }
}