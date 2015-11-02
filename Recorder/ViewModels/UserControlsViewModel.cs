using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using JetBrains.Annotations;
using Recorder.Models;
using Recorder.Utilities;

namespace Recorder.ViewModels
{
    public class UserControlsViewModel : INotifyPropertyChanged, IClosing, IWindowHandleInitialized, IDisposable
    {
        private List<AbstractPanelViewModel> Panels { get; set; }
        private ICommand _showOutputCommand;

        public AskExitViewModel AskExitModel { get;  }
        public IssueNotifyModel NotifyIssueModel { get;  }

        public UserControlsViewModel(IProgramSettings settings, ObjectModel objectModel)
        {
            ProgramSettings = settings;

            AskExitModel = new AskExitViewModel();
            NotifyIssueModel = new IssueNotifyModel();
            OutputWindowViewModel = new OutputWindowViewModel { Visibility = Visibility.Visible, Clear = true };

            Recorder = new RecordingEngine(settings, objectModel, OutputWindowViewModel, NotifyIssueModel);
            Combiner = new CombiningEngine(settings, objectModel, OutputWindowViewModel, NotifyIssueModel);

            Recorder.Initialize();
            Combiner.Initialize();

            BarcodeHandler = new BarcodeHandler(settings.BarcodeScannerIdentifiers);

            ConfigurePanels(objectModel);

         
        }

        private void ConfigurePanels(ObjectModel objectModel)
        {
            Panels = new List<AbstractPanelViewModel>
            {
                new BarcodePanelViewModel(this, objectModel, ProgramSettings.ProjectCode) {Visibility = Visibility.Visible},
                new RecordPanelViewModel(this, objectModel) {Visibility = Visibility.Collapsed},
                new FinishPanelViewModel(this, objectModel) {Visibility = Visibility.Collapsed}
            };

            RegisterChildViewModels();
        }

        

        private void RegisterChildViewModels()
        {
            foreach (var child in Panels.Select(p => p as INotifyPropertyChanged).Where(p => p != null))
            {
                WatchForChildPropertyChanges(child);
            }

            WatchForChildPropertyChanges(OutputWindowViewModel);
        }

        private void WatchForChildPropertyChanges(INotifyPropertyChanged child)
        {
            child.PropertyChanged += OnChildPropertyChanged;
        }

        private void OnChildPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (sender is OutputWindowViewModel && e.PropertyName.Equals(nameof(OutputWindowViewModel.Visibility)))
            {
                OnPropertyChanged(nameof(ShowOutputCommand));
            }
        }

        private IProgramSettings ProgramSettings { get; }

        public string Title => $"{ProgramSettings.ProjectCode} Recorder";

        public BarcodePanelViewModel BarcodePanelViewModel => GetPanel<BarcodePanelViewModel>();
        public RecordPanelViewModel RecordPanelViewModel => GetPanel<RecordPanelViewModel>();
        public FinishPanelViewModel FinishPanelViewModel => GetPanel<FinishPanelViewModel>();
        
        public AbstractPanelViewModel ActivePanelModel => Panels.Single(p => p.Visibility == Visibility.Visible);
        public RecordingEngine Recorder { get; }

        public BarcodeHandler BarcodeHandler { get; }

        public CombiningEngine Combiner { get; set; }
        public OutputWindowViewModel OutputWindowViewModel { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        private T GetPanel<T>() where T : AbstractPanelViewModel
        {
            return Panels.Single(p => p.GetType() == typeof (T)) as T;
        }
        

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public async Task ShowPanel<T>() where T : AbstractPanelViewModel
        {
            foreach (var panel in Panels)
            {
                await panel.Initialize();
                panel.Visibility = Visibility.Collapsed;
            }

            GetPanel<T>().Visibility = Visibility.Visible;

            OnPropertyChanged(nameof(ActivePanelModel));
        }

        public ICommand ShowOutputCommand
        {
            get {
                return _showOutputCommand ?? (_showOutputCommand = new RelayCommand(
                    param => ShowOutputWindow(), 
                    param =>EnableShowOutputCommand()));
            }
        }


        
        private bool EnableShowOutputCommand()
        {
            return OutputWindowViewModel.Visibility != Visibility.Visible;
        }

        private void ShowOutputWindow()
        {
            OutputWindowViewModel.Visibility = Visibility.Visible;
        }

        public bool CancelWindowClose()
        {
            if (Recorder.Recording == false)
            {
                return false;
            }

            return AskExitModel.CancelWindowClose();
        }
        
        public void WindowHandleInitialized(Visual client)
        {
            BarcodeHandler.Initialize(client, this);
        }

        public void Dispose()
        {
            Recorder?.Dispose();
            Combiner?.Dispose();
            BarcodeHandler?.Dispose();
        }
    }
}