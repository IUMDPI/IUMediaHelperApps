using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Packager.Exceptions;
using Packager.Extensions;
using Packager.Models;
using Packager.Models.FileModels;
using Packager.Observers;
using Packager.Processors;
using Packager.Providers;
using Packager.Utilities;

namespace Packager.Engine
{
    public class StandardEngine : IEngine
    {
        private readonly Dictionary<string, IProcessor> _processors;
        private readonly IDependencyProvider _dependencyProvider;
        

        public StandardEngine(
            Dictionary<string, IProcessor> processors,
            IDependencyProvider dependencyProvider)
        {
            _processors = processors;
            _dependencyProvider = dependencyProvider;
        }

        private IObserverCollection Observers
        {
            get { return _dependencyProvider.Observers; }
        }

        private IProgramSettings ProgramSettings
        {
            get { return _dependencyProvider.ProgramSettings; }
        }

        private IDirectoryProvider DirectoryProvider
        {
            get { return _dependencyProvider.DirectoryProvider; }
        }

        private IFileProvider FileProvider
        {
            get { return _dependencyProvider.FileProvider; }
        }

        private IProcessRunner ProcessRunner { get { return _dependencyProvider.ProcessRunner; } }

        public async Task Start()
        {
            try
            {
                WriteHelloMessage();

                await LogConfiguration();

                ProgramSettings.Verify();
                
                // this factory will assign each extension
                // to the appropriate file model
                var factory = new FileModelFactory(_processors.Keys);

                // want to get all files in the input directory
                // and convert them to file models (via out file model factory)
                // and then take all of the files that are valid
                // and start with the correct project code
                // and then group them by barcode
                var objectGroups = DirectoryProvider.EnumerateFiles(ProgramSettings.InputDirectory)
                    .Select(p => factory.GetModel(p))
                    .Where(f => f.IsValid())
                    .Where(f => f.BelongsToProject(ProgramSettings.ProjectCode))
                    .GroupBy(f => f.BarCode).ToList();
                
                Observers.Log("Found {0} objects to process", objectGroups.Count());
               
                var results = new Dictionary<string, bool>();
                
                // now we want to get the processor for each group
                // and process the files for the group
                foreach (var group in objectGroups)
                {
                    results[group.Key] = await ProcessFile(group);
                }

                WriteResultsMessage(results);
                WriteGoodbyeMessage();
            }
            catch (Exception ex)
            {
                Observers.LogEngineIssue(ex);
            }
        }

        public void AddObserver(IObserver observer)
        {
            if (Observers.Any(instance => instance.GetType() == observer.GetType()))
            {
                return;
            }

            Observers.Add(observer);
        }

        private async Task<bool> ProcessFile(IGrouping<string, AbstractFileModel> group)
        {
            var processor = GetProcessor(group);
            return await processor.ProcessFile(group);
        }

        private IProcessor GetProcessor(IEnumerable<AbstractFileModel> group)
        {
            // for each model in the group
            // take those that have extensions associated with a processor
            // and group them by that extension
            var validExtensions = group
                .Where(m => _processors.Keys.Contains(m.Extension))
                .GroupBy(m => m.Extension).ToList();

            // if we have no groups or if we have more than one group, we have a problem
            if (validExtensions.Count() != 1)
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
            Observers.Log("BWF MetaEdit path: {0}", ProgramSettings.BWFMetaEditPath.ToDefaultIfEmpty("[not set]"));
            Observers.Log("BWF MetaEdit version: {0}", FileProvider.GetFileVersion(ProgramSettings.BWFMetaEditPath).ToDefaultIfEmpty("[not available]") );
            Observers.Log("");
            Observers.Log("FFMPEG path: {0}", ProgramSettings.FFMPEGPath.ToDefaultIfEmpty("[not set]"));
            Observers.Log("FFMPEG version: {0}", (await GetFfmpegVersion(ProgramSettings.FFMPEGPath)).ToDefaultIfEmpty("[not available]"));
            Observers.Log("FFMPeg audio production args: {0}", ProgramSettings.FFMPEGAudioProductionArguments.ToDefaultIfEmpty("[not set]"));
            Observers.Log("FFMPeg audio access args: {0}", ProgramSettings.FFMPEGAudioAccessArguments.ToDefaultIfEmpty("[not set]"));
            Observers.EndSection(sectionKey);
        }

        private async Task<string> GetFfmpegVersion(string path)
        {
            try
            {
                if (!FileProvider.FileExists(path))
                {
                    return "";
                }

                var info = new ProcessStartInfo(ProgramSettings.FFMPEGPath) { Arguments = "-version"};
                var result = await ProcessRunner.Run(info);
                
                var parts = result.StandardOutput.Split(' ');
                return parts[2];
            }
            catch (Exception e)
            {
                return "";
            }
        }

        private void WriteResultsMessage(Dictionary<string, bool> results)
        {
            Observers.Log("");
            if (!results.Any())
            {
                return;
            }
            
            var inError = results.Where(r => r.Value == false).Select(r => r.Key).ToList();
            var success = results.Where(r => r.Value).Select(r => r.Key).ToList();

            Observers.Log("Successfully processed {0} objects.", success.Count);

            foreach (var barcode in inError)
            {
                Observers.Log("Could not process {0}", barcode);
            }
        }
    }
}