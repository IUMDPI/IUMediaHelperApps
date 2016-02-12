using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Packager.Exceptions;
using Packager.Extensions;
using Packager.Factories;
using Packager.Models.FileModels;
using Packager.Models.SettingsModels;
using Packager.Observers;
using Packager.Processors;
using Packager.Providers;
using Packager.Validators;

namespace Packager.Engine
{
    public class StandardEngine : IEngine
    {
        private readonly IDependencyProvider _dependencyProvider;
        private readonly Dictionary<string, IProcessor> _processors;

        public StandardEngine(
            Dictionary<string, IProcessor> processors,
            IDependencyProvider dependencyProvider)
        {
            _processors = processors;
            _dependencyProvider = dependencyProvider;
        }

        private IObserverCollection Observers => _dependencyProvider.Observers;
        private IProgramSettings ProgramSettings => _dependencyProvider.ProgramSettings;
        private IDirectoryProvider DirectoryProvider => _dependencyProvider.DirectoryProvider;
        private IValidatorCollection ValidatorCollection => _dependencyProvider.ValidatorCollection;

        public async Task Start()
        {
            try
            {
                var results = new Dictionary<string, ValidationResult>();

                WriteHelloMessage();

                await LogConfiguration();
                ValidateDependencyProvider();

                await CleanupOldFiles();

                // Get the processor for each group
                // and process the files for the group
                foreach (var group in GetObjectGroups())
                {
                    results[group.Key] = await ProcessObject(group);
                }

                TurnOffObjectObservers();
                WriteResultsMessage(results);
            }
            catch (Exception ex)
            {
                Observers.LogEngineIssue(ex);
            }

            WriteGoodbyeMessage();
        }

        public void AddObserver(IObserver observer)
        {
            if (Observers.Any(instance => instance.GetType() == observer.GetType()))
            {
                return;
            }

            Observers.Add(observer);
        }

        private async Task CleanupOldFiles()
        {
            await _dependencyProvider.SuccessFolderCleaner.DoCleaning();
        }

        private void ValidateDependencyProvider()
        {
            var result = ValidatorCollection.Validate(_dependencyProvider);
            if (result.Succeeded == false)
            {
                throw new ProgramSettingsException(result.Issues);
            }
        }

        private IEnumerable<IGrouping<string, AbstractFile>> GetObjectGroups()
        {
            // want to get all files in the input directory
            // and convert them to file models (via out file model factory)
            // and then take all of the files that are valid
            // and start with the correct project code
            // and then group them by barcode
            var result = DirectoryProvider.EnumerateFiles(ProgramSettings.InputDirectory)
                .Select(FileModelFactory.GetModel)
                .Where(f => f.IsValid())
                .Where(f => f.BelongsToProject(ProgramSettings.ProjectCode))
                .GroupBy(f => f.BarCode).ToList();

            Observers.Log("Found {0} to process", result.ToSingularOrPlural("object", "objects"));

            return result;
        }

        private async Task<ValidationResult> ProcessObject(IGrouping<string, AbstractFile> group)
        {
            var processor = GetProcessor(group);
            return await processor.ProcessObject(group);
        }

        private IProcessor GetProcessor(IEnumerable<AbstractFile> group)
        {
            // for each model in the group
            // take those that have extensions associated with a processor
            // and group them by that extension
            var validExtensions = group
                .Where(m => _processors.Keys.Contains(m.Extension))
                .GroupBy(m => m.Extension).ToList();

            // if we have no groups or if we have more than one group, we have a problem
            if (validExtensions.Count != 1)
            {
                throw new DetermineProcessorException("Can not determine extension for file batch");
            }

            return _processors[validExtensions.First().Key];
        }

        private void WriteHelloMessage()
        {
            Observers.Log("Starting {0} (version {1})", DateTime.Now, Assembly.GetExecutingAssembly().GetName().Version);
        }

        private void WriteGoodbyeMessage()
        {
            Observers.Log("Completed {0}", DateTime.Now);
        }

        private async Task LogConfiguration()
        {
            var sectionKey = Observers.BeginSection("Configuration:");

            Observers.Log("Project code: {0}", ProgramSettings.ProjectCode.ToDefaultIfEmpty("[not set]"));
            Observers.Log("Web-service host: {0}", ProgramSettings.WebServiceUrl.ToDefaultIfEmpty("[not set]"));
            Observers.Log("");
            Observers.Log("Input folder: {0}", ProgramSettings.InputDirectory.ToDefaultIfEmpty("[not set]"));
            Observers.Log("Processing folder: {0}", ProgramSettings.ProcessingDirectory.ToDefaultIfEmpty("[not set]"));
            Observers.Log("Dropbox folder: {0}", ProgramSettings.DropBoxDirectoryName.ToDefaultIfEmpty("[not set]"));
            Observers.Log("Success folder: {0}", ProgramSettings.SuccessDirectoryName.ToDefaultIfEmpty("[not set]"));
            Observers.Log("Error folder: {0}", ProgramSettings.ErrorDirectoryName.ToDefaultIfEmpty("[not set]"));
            Observers.Log("");
            Observers.Log("BWF MetaEdit path: {0}",
                _dependencyProvider.MetaEditRunner.BwfMetaEditPath.ToDefaultIfEmpty("[not set]"));
            Observers.Log("BWF MetaEdit version: {0}",
                (await _dependencyProvider.MetaEditRunner.GetVersion()).ToDefaultIfEmpty("[not available]"));
            Observers.Log("Unit prefix: {0}",
                _dependencyProvider.ProgramSettings.UnitPrefix.ToDefaultIfEmpty("[not set]"));
            Observers.Log("");
            Observers.Log("FFMPEG path: {0}",
                _dependencyProvider.AudioFFMPEGRunner.FFMPEGPath.ToDefaultIfEmpty("[not set]"));
            Observers.Log("FFMPEG version: {0}",
                (await _dependencyProvider.AudioFFMPEGRunner.GetFFMPEGVersion()).ToDefaultIfEmpty("[not available]"));
            Observers.Log("FFMPeg audio production args: {0}",
                _dependencyProvider.AudioFFMPEGRunner.ProdOrMezzArguments.ToDefaultIfEmpty("[not set]"));
            Observers.Log("FFMPeg audio access args: {0}",
                _dependencyProvider.AudioFFMPEGRunner.AccessArguments.ToDefaultIfEmpty("[not set]"));
            Observers.Log("FFMPeg video mezzanine args: {0}",
                _dependencyProvider.VideoFFMPEGRunner.ProdOrMezzArguments.ToDefaultIfEmpty("[not set]"));
            Observers.Log("FFMPeg video access args: {0}",
                _dependencyProvider.VideoFFMPEGRunner.AccessArguments.ToDefaultIfEmpty("[not set]"));
            Observers.Log("");
            Observers.Log("FFProbe path: {0}",
                _dependencyProvider.FFProbeRunner.FFProbePath.ToDefaultIfEmpty("[not set]"));
            Observers.Log("FFProbe version: {0}",
                (await _dependencyProvider.FFProbeRunner.GetVersion()).ToDefaultIfEmpty("[not available]"));
            Observers.Log("FFProbe video QC args: {0}",
                _dependencyProvider.FFProbeRunner.VideoQualityControlArguments.ToDefaultIfEmpty("[not set]"));
            Observers.Log("");
            Observers.Log("Success folder cleaning: {0}",
                _dependencyProvider.SuccessFolderCleaner.Enabled
                    ? $"remove items older than {_dependencyProvider.SuccessFolderCleaner.ConfiguredInterval}"
                    : "disabled");

            Observers.EndSection(sectionKey);
        }

        private void WriteResultsMessage(Dictionary<string, ValidationResult> results)
        {
            if (!results.Any())
            {
                Observers.Log("");
                return;
            }

            var section = Observers.BeginSection("Results Summary:");

            Observers.Log("Found {0} to process.", results.ToSingularOrPlural("object", "objects"));

            var inError = results.Where(r => r.Value.Result == false).ToList();
            var success = results.Where(r => r.Value.Result).ToList();

            LogObjectResults(success, $"Successfully processed {success.ToSingularOrPlural("object", "objects")}:");
            LogObjectResults(inError, $"Could not process {inError.ToSingularOrPlural("object", "objects")}:");

            Observers.EndSection(section);
        }

        private void LogObjectResults(List<KeyValuePair<string, ValidationResult>> results, string header)
        {
            if (results.Any() == false)
            {
                return;
            }

            var sectionKey = Observers.BeginSection(header);

            foreach (var result in results)
            {
                Observers.Log("  {0}", result.Key);
            }

            Observers.EndSection(sectionKey);
        }

        private void TurnOffObjectObservers()
        {
            foreach (var observer in Observers.Select(o => o as ObjectNLogObserver).Where(o => o != null))
            {
                observer.ClearBarcode();
            }
        }
    }
}