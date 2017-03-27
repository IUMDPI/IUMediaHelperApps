using System.Collections.Generic;
using System.Linq;
using Common.Models;
using Packager.Extensions;
using Packager.Models.FileModels;
using Packager.Models.PlaceHolderConfigurations;

namespace Packager.Factories
{
    public interface IPlaceHolderFactory
    {
        List<AbstractFile> GetPlaceHoldersToAdd(string format, List<AbstractFile> fileModels);
    }

    public class PlaceHolderFactory : IPlaceHolderFactory
    {
        private readonly Dictionary<string, IPlaceHolderConfiguration> _knownConfigurations = new Dictionary<string, IPlaceHolderConfiguration>
        {
            // standard audio
            { "audiocassette", new StandardAudioPlaceHolderConfiguration() },
            {"open reel audio tape", new StandardAudioPlaceHolderConfiguration() },
            {"lp", new StandardAudioPlaceHolderConfiguration() },
            {"cd-r", new StandardAudioPlaceHolderConfiguration() },
            {"45", new StandardAudioPlaceHolderConfiguration() },
            // pres-int audio
            { "lacquer disc", new PresIntAudioPlaceHolderConfiguration()},
            {"78", new PresIntAudioPlaceHolderConfiguration() },
            // standard video
            { "vhs", new StandardVideoPlaceHolderConfiguration()},
            {"betacam:anamorphic", new StandardVideoPlaceHolderConfiguration() },
            {"dat", new StandardVideoPlaceHolderConfiguration() },
            {"1-inch open reel video tape", new StandardVideoPlaceHolderConfiguration() },
            {"8mm video", new StandardVideoPlaceHolderConfiguration() },
            {"betacam", new StandardVideoPlaceHolderConfiguration() },
            {"8mm video:quadaudio", new StandardVideoPlaceHolderConfiguration() },
            {"u-matic", new StandardAudioPlaceHolderConfiguration() },
            {"betamax", new StandardVideoPlaceHolderConfiguration() },

        };

        private readonly List<FileUsages> _allRequiredUsages = new List<FileUsages>
        {
            FileUsages.PreservationMaster,
            FileUsages.PreservationIntermediateMaster,
            FileUsages.ProductionMaster,
            FileUsages.MezzanineFile,
            FileUsages.ProductionMaster,
            FileUsages.AccessFile
        };

        public List<AbstractFile> GetPlaceHoldersToAdd(string format, List<AbstractFile> fileModels)
        {
            var configuration = _knownConfigurations.ContainsKey(format.ToLowerInvariant())
                ? _knownConfigurations[format.ToLowerInvariant()]
                : GetDynamicConfiguration(fileModels);

            return configuration.GetPlaceHoldersToAdd(fileModels);
        }

        private IPlaceHolderConfiguration GetDynamicConfiguration(List<AbstractFile> fileModels)
        {
            var firstSequence = fileModels.Where(fm => fm.SequenceIndicator == 1)
                .ToList();

            var requiredUsages = new HashSet<FileUsages>();
            foreach (var fileModel in firstSequence)
            {
                if (!_allRequiredUsages.Contains(fileModel.FileUsage))
                {
                    continue;
                }

                requiredUsages.Add(fileModel.FileUsage);
            }

            var application = GetUsageApplication(firstSequence);

            return new DynamicPlaceholderConfiguration(requiredUsages.ToList(), application);
        }

        private FileModelFactory.UsageApplications GetUsageApplication(List<AbstractFile> fileModels)
        {
            var presOrPresIntModel = fileModels.GetPreservationOrIntermediateModel();
            return presOrPresIntModel is AudioPreservationFile ||
                   presOrPresIntModel is AudioPreservationIntermediateFile
                ? FileModelFactory.UsageApplications.Audio
                : FileModelFactory.UsageApplications.Video;
        }
    }
}
