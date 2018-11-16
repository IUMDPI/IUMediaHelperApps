using Packager.Extensions;
using Packager.Models.SettingsModels;
using Packager.Utilities.ProcessRunners;

namespace Packager.Factories.FFMPEGArguments
{
    public class AudioFFMPEGArgumentsGenerator : AbstractFFMPEGArgumentGenerator
    {
        private const string RequiredProductionBextArguments = "-write_bext 1";
        private const string RequiredProductionRiffArguments = "-rf64 auto";
        private const string RequiredAccessArguments = "-map_metadata -1";
        
        public AudioFFMPEGArgumentsGenerator(IProgramSettings programSettings)
        {
            NormalizingArguments = new ArgumentBuilder("-acodec copy -write_bext 1 -rf64 auto -map_metadata -1");
            AccessArguments = new ArgumentBuilder(AddRequiredAccessArguments(programSettings.FFMPEGAudioAccessArguments));
            ProdOrMezzArguments = new ArgumentBuilder(AddRequiredProdArguments(programSettings.FFMPEGAudioProductionArguments));
        }

        private static string AddRequiredAccessArguments(string arguments)
        {
            if (arguments.IsNotSet())
            {
                return arguments;
            }

            
            if (!arguments.ToLowerInvariant().Contains(RequiredAccessArguments.ToLowerInvariant()))
            {
                arguments = $"{arguments} {RequiredAccessArguments}";
            }

            return arguments;
        }

        private static string AddRequiredProdArguments(string arguments)
        {
            if (arguments.IsNotSet())
            {
                return arguments;
            }

            if (!arguments.ToLowerInvariant().Contains(RequiredProductionBextArguments.ToLowerInvariant()))
            {
                arguments = $"{arguments} {RequiredProductionBextArguments}";
            }

            if (!arguments.ToLowerInvariant().Contains(RequiredProductionRiffArguments.ToLowerInvariant()))
            {
                arguments = $"{arguments} {RequiredProductionRiffArguments}";
            }

            return arguments;
        }
    }
}