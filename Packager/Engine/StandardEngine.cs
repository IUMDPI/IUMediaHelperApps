﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using Common.Models;
using Packager.Exceptions;
using Packager.Extensions;
using Packager.Factories;
using Packager.Models.EmailMessageModels;
using Packager.Models.FileModels;
using Packager.Models.ProgramArgumentsModels;
using Packager.Models.SettingsModels;
using Packager.Observers;
using Packager.Processors;
using Packager.Providers;
using Packager.UserInterface;
using Packager.Utilities.Configuration;
using Packager.Utilities.Email;
using Packager.Utilities.FileSystem;
using Packager.Validators;

namespace Packager.Engine
{
    public class StandardEngine : IEngine
    {
        private IViewModel ViewModel { get; }
        private Dictionary<string, IProcessor> Processors { get; }
        private IProgramSettings ProgramSettings { get; }
        private IProgramArguments ProgramArguments { get; }
        private IDirectoryProvider DirectoryProvider { get; }
        private IObserverCollection Observers { get; }
        private IValidatorCollection ValidatorCollection { get; }
        private ISuccessFolderCleaner SuccessFolderCleaner { get; }
        private IConfigurationLogger ConfigurationLogger { get; }
        private ISystemInfoProvider SystemInfoProvider { get; }
        private IEmailSender EmailSender { get; }


        public StandardEngine(
            Dictionary<string, IProcessor> processors, 
            IViewModel viewModel,
            IProgramSettings programSettings, 
            IProgramArguments programArguments,
            IDirectoryProvider directoryProvider,
            IValidatorCollection validatorCollection, 
            ISuccessFolderCleaner successFolderCleaner,
            IConfigurationLogger configurationLogger,
            ISystemInfoProvider systemInfoProvider,
            IEmailSender emailSender,
            IObserverCollection observerCollection)
        {
            ViewModel = viewModel;
            ProgramSettings = programSettings;
            ProgramArguments = programArguments;
            DirectoryProvider = directoryProvider;
            ValidatorCollection = validatorCollection;
            SuccessFolderCleaner = successFolderCleaner;
            ConfigurationLogger = configurationLogger;
            SystemInfoProvider = systemInfoProvider;
            EmailSender = emailSender;
            Observers = observerCollection;
            Processors = processors;
        }
        
        public async Task Start(CancellationToken cancellationToken)
        {
            EngineExitCodes exitCode;

            try
            {
                var results = new Dictionary<string, ValidationResult>();
                WriteHelloMessage();

                await ConfigurationLogger.Log();
                ValidateSettings();

                await CleanupOldFiles();

                cancellationToken.Register(cancellationToken.ThrowIfCancellationRequested);

                // Get the processor for each group
                // and process the files for the group
                var groupings = GetObjectGroups();
                for(var index =0; index<groupings.Length; index++)
                {
                    var group = groupings[index];
                    UpdateCancelBanner($"Processing object {index+1} of {groupings.Length}: {group.Key}");
                    results[group.Key] = await ProcessObject(group, cancellationToken);
                    if (cancellationToken.IsCancellationRequested)
                    {
                        break;
                    }
                }

                HideCancelBanner();
                TurnOffObjectObservers();

                WriteResultsMessage(groupings, results, cancellationToken);
                WriteResultsReport(groupings, results, cancellationToken);

                SendSuccessEmail(results);
                exitCode = GetExitCode(results);
            }
            catch (Exception ex)
            {
                Observers.LogEngineIssue(ex);
                WriteResultsReport(ex);
                exitCode = EngineExitCodes.EngineIssue;
            }

            WriteGoodbyeMessage();

            ExitIfNonInteractive(exitCode);
        }

        private void ExitIfNonInteractive(EngineExitCodes exitCode)
        {
            if (ProgramArguments.Interactive)
            {
                return;
            }

            SystemInfoProvider.ExitApplication(exitCode);
        }

        private EngineExitCodes GetExitCode(Dictionary<string, ValidationResult> results)
        {
            return results.Any(r => r.Value.Result == false) 
                ? EngineExitCodes.ProcessingIssue
                : EngineExitCodes.Success; // success
        }

        private void SendSuccessEmail(Dictionary<string, ValidationResult> results)
        {
            var succeededBarCodes = results.Where(r => r.Value.Result).Select(r=>r.Key).ToArray();
            if (succeededBarCodes.Any() == false)
            {
                return;
            }

            if (ProgramSettings.SuccessNotifyEmailAddresses.Any() == false)
            {
                return;
            }

            var message = new SuccessEmailMessage(
                succeededBarCodes, 
                ProgramSettings.SuccessNotifyEmailAddresses, 
                ProgramSettings.FromEmailAddress, 
                SystemInfoProvider.MachineName,
                SystemInfoProvider.CurrentSystemLogPath);

            EmailSender.Send(message);
        }

        private void UpdateCancelBanner(string message)
        {
            ViewModel.Processing = true;
            ViewModel.ProcessingMessage = message;
        }

        private void HideCancelBanner()
        {
            ViewModel.Processing = false;
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
            await SuccessFolderCleaner.DoCleaning();
        }

        private void ValidateSettings()
        {
            var result = ValidatorCollection.Validate(ProgramSettings);
            if (result.Succeeded == false)
            {
                throw new ProgramSettingsException(result.Issues);
            }
        }

        private IGrouping<string, AbstractFile>[] GetObjectGroups()
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

            return result.ToArray();
        }

        private async Task<ValidationResult> ProcessObject(IGrouping<string, AbstractFile> group, CancellationToken cancellationToken)
        {
            var processor = GetProcessor(group);
            return await processor.ProcessObject(group, cancellationToken);
        }

        private IProcessor GetProcessor(IEnumerable<AbstractFile> group)
        {
            // for each model in the group
            // take those that have extensions associated with a processor
            // and group them by that extension
            var validExtensions = group
                .Where(m => Processors.Keys.Contains(m.Extension))
                .GroupBy(m => m.Extension).ToList();

            // if we have no groups or if we have more than one group, we have a problem
            if (validExtensions.Count != 1)
            {
                throw new DetermineProcessorException("Can not determine extension for file batch");
            }

            return Processors[validExtensions.First().Key];
        }

        private void WriteHelloMessage()
        {
            Observers.Log("Starting {0} (version {1})", DateTime.Now, Assembly.GetExecutingAssembly().GetName().Version);
        }

        private void WriteGoodbyeMessage()
        {
            Observers.Log("Completed {0}", DateTime.Now);
        }
        
        private void WriteResultsMessage(IEnumerable<IGrouping<string, AbstractFile>> groupings, Dictionary<string, ValidationResult> results, CancellationToken cancellationToken)
        {
            if (!results.Any())
            {
                Observers.Log("");
                return;
            }

            var section = Observers.BeginSection("Results Summary:");

            Observers.Log("Found {0} to process.", groupings.ToSingularOrPlural("object", "objects"));

            var inError = results.Where(r => r.Value.Result == false).ToList();
            var success = results.Where(r => r.Value.Result).ToList();

            LogObjectResults(success, $"Successfully processed {success.ToSingularOrPlural("object", "objects")}:");
            LogObjectResults(inError, $"Could not process {inError.ToSingularOrPlural("object", "objects")}:");

            if (cancellationToken.IsCancellationRequested)
            {
                Observers.Log($"User canceled operation while processing {results.Last().Key}");
            }

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

        private void WriteResultsReport(IEnumerable<IGrouping<string, AbstractFile>> groupings,
            Dictionary<string, ValidationResult> results, CancellationToken cancellationToken)
        {
            var report = new PackagerReport
            {
                Timestamp = DateTime.UtcNow,
                Succeeded = results.All(r=>r.Value.Result),
                ObjectReports = results.Select(r=>new PackagerObjectReport
                {
                    Barcode = r.Key,
                    Succeeded = r.Value.Result,
                    Issue = r.Value.Issue,
                    Timestamp = r.Value.Timestamp
                }).ToList()
            };

            report.Save(ProgramSettings.LogDirectoryName);
        }

        private void WriteResultsReport(Exception e)
        {
            var report = new PackagerReport
            {
                Succeeded = false,
                Issue = e.Message,
                Timestamp = DateTime.UtcNow
            };
            report.Save(ProgramSettings.LogDirectoryName);
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