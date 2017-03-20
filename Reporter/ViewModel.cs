using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
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

        private delegate void SelectionChangedDelegate(AbstractReportEntry entry);

        private event SelectionChangedDelegate SelectionChanged;

        private readonly List<IReportRenderer> _reportReaders;
        private AbstractReportEntry _selectedEntry;
        private string _windowTitle;
        private bool _initializing;

        public string WindowTitle
        {
            get { return _windowTitle;}
            set { _windowTitle = value; OnPropertyChanged(); }
        }

        public bool Initializing
        {
            get { return _initializing;}
            set { _initializing = value; OnPropertyChanged(); }
        }

        private ICommand _selectFolderCommand;
        private ICommand _refreshReportsCommand;

        public BindingList<AbstractReportEntry> Reports { get; }

        public AbstractReportEntry SelectedReport
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
                       (_selectFolderCommand = new AsyncRelayCommand(
                           async action => await SelectFolder()));
            }
        }

        public ICommand RefreshReportsList
        {
            get
            {
                return _refreshReportsCommand ??
                       (_refreshReportsCommand = new AsyncRelayCommand(
                           async action => await InitializeReportsList()));
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

        public ViewModel(ProgramSettings programSettings, List<IReportRenderer> reportReaders, ILogPanelViewModel logPanelViewModel)
        {
            ProgramSettings = programSettings;
            WindowTitle = $"{programSettings.ProjectCode} Reporter";
            Reports = new BindingList<AbstractReportEntry>
            {
                RaiseListChangedEvents = true
            };
            Reports.ListChanged += (sender, args) => { OnPropertyChanged(nameof(Reports)); };
            LogPanelViewModel = logPanelViewModel;
            _reportReaders = reportReaders;
            SelectionChanged += SelectionChangedHandler;
        }

        private IReportRenderer GetReaderForReport(AbstractReportEntry report)
        {
            return _reportReaders.SingleOrDefault(r => r.CanRender(report));
        }

        private async void SelectionChangedHandler(
            AbstractReportEntry entry)
        {
            LogPanelViewModel.Clear();

            var reader = GetReaderForReport(entry);
            if (reader == null)
            {
                LogPanelViewModel.InsertLine($"This operationReport cannot be opened.\n\nPlease try a different operationReport.");
                return;
            }

            await reader.Render(entry);
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

        private async Task<List<AbstractReportEntry>> GetReports()
        {
            var results = new List<AbstractReportEntry>();
            foreach (var reader in _reportReaders)
            {
                results.AddRange(await reader.GetReports());
            }

            return results;
        }

        private async Task InitializeReportsList()
        {
            var started = DateTime.Now;

            if (Application.Current.Dispatcher.CheckAccess() == false)
            {
                await Application.Current.Dispatcher.InvokeAsync(async () => await InitializeReportsList());
                return;
            }

            Initializing = true;
            Reports.Clear();

            var entries = (await GetReports().ConfigureAwait(false)).OrderByDescending(e => e.Timestamp).ToList();

            SetEntries(entries);

            var elapsed = DateTime.Now.Subtract(started);
            var remaining = (int)(2000 - elapsed.TotalMilliseconds);
            if (remaining < 0) remaining = 0;
            await Task.Delay(remaining).ContinueWith(action =>
            {
                Initializing = false;
            });
        }

        private void SetEntries(List<AbstractReportEntry> entries)
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

