using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Common.Annotations;
using Common.Extensions;
using Common.Models;
using Common.UserInterface.ViewModels;
using ICSharpCode.AvalonEdit;
using Reporter.Models;
using Reporter.Utilities;

namespace Reporter
{
    public class ViewModel : INotifyPropertyChanged
    {
        private delegate void SelectionChangedDelegate(ReportEntry entry);

        private event SelectionChangedDelegate SelectionChanged;

        private readonly IReportReader _reportReader;
        private ReportEntry _selectedEntry;

        public BindingList<ReportEntry> Reports { get; }

        public ReportEntry SelectedReport
        {
            get { return _selectedEntry; }
            set { _selectedEntry = value; OnPropertyChanged();
                SelectionChanged?.Invoke(value);
            }
        }

        public ILogPanelViewModel LogPanelViewModel { get; }

        public ViewModel(IReportReader reportReader, ILogPanelViewModel logPanelViewModel)
        {
            Reports = new BindingList<ReportEntry>();
            LogPanelViewModel = logPanelViewModel;
            _reportReader = reportReader;
            SelectionChanged+=SelectionChangedHandler;
        }

        private async void SelectionChangedHandler(ReportEntry entry)
        {
            var report = await _reportReader.GetReport<PackagerReport>("c:\\work\\logs", entry);
            if (report == null)
            {
                LogPanelViewModel.InsertLine("No reports present");
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
                    LogPanelViewModel.InsertLine($"Issue: {result.Issue}");
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
            LogPanelViewModel.Initialize(reportText);

            await InitializeReportsList();
        }

        private async Task InitializeReportsList()
        {
            Reports.Clear();

            var entries = (await _reportReader.GetReports("c:\\work\\logs").ConfigureAwait(false)).OrderBy(e => e.Filename).ToList();

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

