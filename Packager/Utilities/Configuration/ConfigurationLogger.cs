using System.Threading.Tasks;
using Packager.Extensions;
using Packager.Models.SettingsModels;
using Packager.Observers;
using Packager.Utilities.FileSystem;
using Packager.Utilities.ProcessRunners;

namespace Packager.Utilities.Configuration
{
    public interface IConfigurationLogger
    {
        Task Log();
    }

    public class ConfigurationLogger:IConfigurationLogger
    {
        private IProgramSettings ProgramSettings { get; }
        private IBwfMetaEditRunner MetaEditRunner { get; }
        private IFFMPEGRunner FFMPEGRunner { get; }
        private IFFProbeRunner FFProbeRunner { get; }
        private ISuccessFolderCleaner SuccessFolderCleaner { get; set; }
        private IObserverCollection Observers { get; }

        public ConfigurationLogger(IProgramSettings programSettings, IBwfMetaEditRunner metaEditRunner, IFFMPEGRunner ffmpegRunner, IFFProbeRunner ffProbeRunner, ISuccessFolderCleaner successFolderCleaner, IObserverCollection observers)
        {
            ProgramSettings = programSettings;
            MetaEditRunner = metaEditRunner;
            FFMPEGRunner = ffmpegRunner;
            FFProbeRunner = ffProbeRunner;
            SuccessFolderCleaner = successFolderCleaner;
            Observers = observers;
        }

        public async Task Log()
        {
            var sectionKey = Observers.BeginSection("Configuration:");

            Observers.Log("Project code: {0}", ProgramSettings.ProjectCode.ToDefaultIfEmpty("[not set]"));
            Observers.Log("Digitizing entity: {0}", ProgramSettings.DigitizingEntity.ToDefaultIfEmpty("[not set]"));
            Observers.Log("Web-service host: {0}", ProgramSettings.WebServiceUrl.ToDefaultIfEmpty("[not set]"));
            Observers.Log("");
            Observers.Log("Input folder: {0}", ProgramSettings.InputDirectory.ToDefaultIfEmpty("[not set]"));
            Observers.Log("Processing folder: {0}", ProgramSettings.ProcessingDirectory.ToDefaultIfEmpty("[not set]"));
            Observers.Log("Dropbox folder: {0}", ProgramSettings.DropBoxDirectoryName.ToDefaultIfEmpty("[not set]"));
            Observers.Log("Success folder: {0}", ProgramSettings.SuccessDirectoryName.ToDefaultIfEmpty("[not set]"));
            Observers.Log("Error folder: {0}", ProgramSettings.ErrorDirectoryName.ToDefaultIfEmpty("[not set]"));
            Observers.Log("");
            Observers.Log("BWF MetaEdit path: {0}", MetaEditRunner.BwfMetaEditPath.ToDefaultIfEmpty("[not set]"));
            Observers.Log("BWF MetaEdit version: {0}", (await MetaEditRunner.GetVersion()).ToDefaultIfEmpty("[not available]"));
            Observers.Log("Unit prefix: {0}", ProgramSettings.UnitPrefix.ToDefaultIfEmpty("[not set]"));
            Observers.Log("");
            Observers.Log("FFMPEG path: {0}",ProgramSettings.FFMPEGPath.ToDefaultIfEmpty("[not set]"));
            Observers.Log("FFMPEG version: {0}",
                (await FFMPEGRunner.GetFFMPEGVersion()).ToDefaultIfEmpty("[not available]"));
            Observers.Log("FFMPeg audio production args: {0}",
                ProgramSettings.FFMPEGAudioProductionArguments.ToDefaultIfEmpty("[not set]"));
            Observers.Log("FFMPeg audio access args: {0}",
                ProgramSettings.FFMPEGAudioAccessArguments.ToDefaultIfEmpty("[not set]"));
            Observers.Log("FFMPeg video mezzanine args: {0}",
                ProgramSettings.FFMPEGVideoMezzanineArguments.ToDefaultIfEmpty("[not set]"));
            Observers.Log("FFMPeg video access args: {0}",
                ProgramSettings.FFMPEGVideoAccessArguments.ToDefaultIfEmpty("[not set]"));
            Observers.Log("");
            Observers.Log("FFProbe path: {0}",
                ProgramSettings.FFProbePath.ToDefaultIfEmpty("[not set]"));
            Observers.Log("FFProbe version: {0}",
                (await FFProbeRunner.GetVersion()).ToDefaultIfEmpty("[not available]"));
            Observers.Log("FFProbe video QC args: {0}",
                FFProbeRunner.VideoQualityControlArguments.ToDefaultIfEmpty("[not set]"));
            Observers.Log("");
            Observers.Log("Success folder cleaning: {0}", SuccessFolderCleaner.Enabled
                    ? $"remove items older than {SuccessFolderCleaner.ConfiguredInterval}"
                    : "disabled");
            Observers.EndSection(sectionKey);
        }
    }
}
