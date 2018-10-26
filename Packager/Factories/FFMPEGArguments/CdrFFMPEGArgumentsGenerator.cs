using Packager.Models.SettingsModels;

namespace Packager.Factories.FFMPEGArguments
{
    public class CdrFFMPEGArgumentsGenerator : AudioFFMPEGArgumentsGenerator
    {
        public CdrFFMPEGArgumentsGenerator(IProgramSettings programSettings) : base(programSettings)
        {
            // use normalizing arguments for production master
            ProdOrMezzArguments = NormalizingArguments;
        }
    }
}