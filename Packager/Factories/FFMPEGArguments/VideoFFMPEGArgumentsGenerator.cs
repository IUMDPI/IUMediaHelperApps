using Packager.Models.SettingsModels;
using Packager.Utilities.ProcessRunners;

namespace Packager.Factories.FFMPEGArguments
{
    public class VideoFFMPEGArgumentsGenerator : AbstractFFMPEGArgumentGenerator
    {
        public VideoFFMPEGArgumentsGenerator(IProgramSettings programSettings)
        {
            NormalizingArguments = new ArgumentBuilder("-map 0 -acodec copy -vcodec copy");
            AccessArguments = new ArgumentBuilder(programSettings.FFMPEGAudioAccessArguments);
            ProdOrMezzArguments = new ArgumentBuilder(programSettings.FFMPEGVideoMezzanineArguments);
        }
    }
}