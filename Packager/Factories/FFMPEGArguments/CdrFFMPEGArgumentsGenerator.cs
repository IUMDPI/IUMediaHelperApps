using Packager.Models.SettingsModels;
using Packager.Utilities.ProcessRunners;

namespace Packager.Factories.FFMPEGArguments
{
    public class CdrFFMPEGArgumentsGenerator : AudioFFMPEGArgumentsGenerator
    {
        public CdrFFMPEGArgumentsGenerator(IProgramSettings programSettings) : base(programSettings)
        {
            // use normalizing arguments for production master
            ProdOrMezzArguments = NormalizingArguments;
            AccessArguments = new ArgumentBuilder("-c:a aac -b:a 192k -strict -2 -ar 44100 -map_metadata -1");
        }
    }
}