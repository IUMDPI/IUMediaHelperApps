using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using JetBrains.Annotations;
using Recorder.Models;
using Recorder.Utilities;

namespace Recorder.ViewModels
{
    public class UserControlsViewModel : INotifyPropertyChanged, IClosing, IWindowHandleInitialized, IDisposable
    {
        private readonly List<AbstractPanelViewModel> _panels;

        public UserControlsViewModel(IProgramSettings settings, ObjectModel objectModel)
        {
            Recorder = new RecordingEngine(settings, objectModel);
            Combiner = new CombiningEngine(settings, objectModel);
            BarcodeHandler = new BarcodeHandler(settings.BarcodeScannerIdentifiers);
            ProgramSettings = settings;

            _panels = new List<AbstractPanelViewModel>
            {
                new BarcodePanelViewModel(this, objectModel, settings.ProjectCode) {Visibility = Visibility.Visible},
                new RecordPanelViewModel(this, objectModel) {Visibility = Visibility.Collapsed},
                new FinishPanelViewModel(this, objectModel) {Visibility = Visibility.Collapsed}
            };

            OutputWindowViewModel = new OutputWindowViewModel {Visibility = Visibility.Visible, Clear = true};

            Recorder.OutputWindowViewModel = OutputWindowViewModel;
        }


        private IProgramSettings ProgramSettings { get; }

        public string Title => $"{ProgramSettings.ProjectCode} Recorder";

        public BarcodePanelViewModel BarcodePanelViewModel => GetPanel<BarcodePanelViewModel>();
        public RecordPanelViewModel RecordPanelViewModel => GetPanel<RecordPanelViewModel>();
        public FinishPanelViewModel FinishPanelViewModel => GetPanel<FinishPanelViewModel>();

        public AbstractPanelViewModel ActivePanelModel => _panels.Single(p => p.Visibility == Visibility.Visible);
        public RecordingEngine Recorder { get; }

        public BarcodeHandler BarcodeHandler { get; }

        public CombiningEngine Combiner { get; set; }
        public OutputWindowViewModel OutputWindowViewModel { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        private T GetPanel<T>() where T : AbstractPanelViewModel
        {
            return _panels.Single(p => p.GetType() == typeof (T)) as T;
        }

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public async Task ShowPanel<T>() where T : AbstractPanelViewModel
        {
            foreach (var panel in _panels)
            {
                await panel.Initialize();
                panel.Visibility = Visibility.Collapsed;
            }

            GetPanel<T>().Visibility = Visibility.Visible;

            OnPropertyChanged(nameof(ActivePanelModel));
        }
        
        public bool CancelWindowClose()
        {
            if (Recorder.Recording == false)
            {
                return false;
            }

            var result = MessageBox.Show("You are still recording. Are you sure you want to exit", "Stop Recording and Exit?", MessageBoxButton.YesNo, MessageBoxImage.Warning, MessageBoxResult.No);
            return result != MessageBoxResult.Yes;
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