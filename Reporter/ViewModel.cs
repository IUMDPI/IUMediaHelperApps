﻿using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Common.Annotations;
using Common.Extensions;
using Common.Models;
using Common.UserInterface.Commands;
using Common.UserInterface.ViewModels;
using ICSharpCode.AvalonEdit;
using Microsoft.WindowsAPICodePack.Dialogs;
using Reporter.LineGenerators;
using Reporter.Models;
using Reporter.Utilities;

namespace Reporter
{
    public class ViewModel : INotifyPropertyChanged
    {
        private ProgramSettings ProgramSettings { get; }

        private delegate void SelectionChangedDelegate(ReportEntry entry);

        private event SelectionChangedDelegate SelectionChanged;

        private readonly IReportReader _reportReader;
        private ReportEntry _selectedEntry;
        private string _windowTitle;

        public string WindowTitle
        {
            get { return _windowTitle;}
            set { _windowTitle = value; OnPropertyChanged(); }
        }

        private ICommand _selectFolderCommand;

        public BindingList<ReportEntry> Reports { get; }

        public ReportEntry SelectedReport
        {
            get { return _selectedEntry; }
            set
            {
                _selectedEntry = value; OnPropertyChanged();
                SelectionChanged?.Invoke(value);
            }
        }

        public ILogPanelViewModel LogPanelViewModel { get; }

        public ICommand SelectReportsFolder
        {
            get
            {
                return _selectFolderCommand ??
                       (_selectFolderCommand = new AsyncRelayCommand(async action => await SelectFolder()));
            }
        }

        private async Task SelectFolder()
        {
            if (Application.Current.Dispatcher.CheckAccess() == false)
            {
                await Application.Current.Dispatcher.InvokeAsync(SelectFolder);
                return;
            }

            var dialog = new CommonOpenFileDialog
            {
                Title = "Select reports folder",
                IsFolderPicker = true,
                InitialDirectory = ProgramSettings.ReportFolder,
                AddToMostRecentlyUsedList = false,
                AllowNonFileSystemItems = false,
                DefaultDirectory = ProgramSettings.ReportFolder,
                EnsureFileExists = true,
                EnsurePathExists = true,
                EnsureReadOnly = false,
                EnsureValidNames = true,
                Multiselect = false,
                ShowPlacesList = true
            };

            if (dialog.ShowDialog() != CommonFileDialogResult.Ok)
            {
                return;
            }

            ProgramSettings.ReportFolder = dialog.FileName;
            await InitializeReportsList();
        }

        public ViewModel(ProgramSettings programSettings, IReportReader reportReader, ILogPanelViewModel logPanelViewModel)
        {
            ProgramSettings = programSettings;
            WindowTitle = $"{programSettings.ProjectCode} Reporter";
            Reports = new BindingList<ReportEntry>
            {
                RaiseListChangedEvents = true,
            };
            Reports.ListChanged += (sender, args) => { OnPropertyChanged(nameof(Reports)); };
            LogPanelViewModel = logPanelViewModel;
            _reportReader = reportReader;
            SelectionChanged += SelectionChangedHandler;
        }

        private async void SelectionChangedHandler(ReportEntry entry)
        {
            LogPanelViewModel.Clear();

            var report = await _reportReader.GetReport<PackagerReport>(entry);
            if (report == null)
            {
                LogPanelViewModel.InsertLine($"There are no reports in the {ProgramSettings.ReportFolder} folder.\n\nPlease select a different folder.");
                return;
            }

            LogPanelViewModel.BeginSection("Summary", "Results Summary:");

            LogPanelViewModel.InsertLine($"Started:   {report.Timestamp:MM/dd/yyyy hh:mm tt}");
            LogPanelViewModel.InsertLine($"Completed: {report.Timestamp.Add(report.Duration):MM/dd/yyyy hh:mm tt}");
            LogPanelViewModel.InsertLine($"Duration:  {report.Duration:hh\\:mm\\:ss}");
            LogPanelViewModel.InsertLine("");
            LogPanelViewModel.InsertLine($"Found {report.ObjectReports.Count.ToSingularOrPlural("object", "objects")} to process.");

            var inError = report.ObjectReports.Where(r => r.Succeeded == false).ToList();
            var success = report.ObjectReports.Where(r => r.Succeeded).ToList();

            LogObjectResults(success, $"Successfully processed {success.ToSingularOrPlural("object", "objects")}:");
            LogObjectResults(inError, $"Could not process {inError.ToSingularOrPlural("object", "objects")}:");

            LogPanelViewModel.EndSection("Summary");
        }

        private void LogObjectResults(List<PackagerObjectReport> results, string header)
        {
            if (results.Any() == false)
            {
                return;
            }

            LogPanelViewModel.BeginSection(header, header);

            foreach (var result in results)
            {
                LogPanelViewModel.InsertLine($"{result.Barcode} ({result.Duration:hh\\:mm\\:ss})");
                if (result.Succeeded == false)
                {
                    LogPanelViewModel.InsertLine("");
                    LogPanelViewModel.InsertLine($"ERROR: {result.Issue}");
                }
            }

            LogPanelViewModel.EndSection(header, header);
        }

        public event PropertyChangedEventHandler PropertyChanged;
        
        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public async Task Initialize(TextEditor reportText)
        {
            LogPanelViewModel.Initialize(reportText, new [] {new OpenObjectLogfileGenerator(ProgramSettings) });

            await InitializeReportsList();
        }

        private async Task InitializeReportsList()
        {
            Reports.Clear();

            var entries = (await _reportReader.GetReports().ConfigureAwait(false)).OrderByDescending(e => e.Timestamp).ToList();

            SetEntries(entries);
        }

        private void SetEntries(List<ReportEntry> entries)
        {
            Application.Current.Dispatcher.InvokeAsync(() =>
            {
                foreach (var entry in entries)
                {
                    Reports.Add(entry);
                }
                SelectedReport = entries.FirstOrDefault();
            });
        }
    }
}
