using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using JetBrains.Annotations;
using Recorder.Models;
using Recorder.Utilities;

namespace Recorder.ViewModels
{
    public class UserControlsViewModel : INotifyPropertyChanged
    {
        private readonly List<AbstractPanelViewModel> _panels;
    
        private readonly IProgramSettings _settings;
       
        public UserControlsViewModel(IProgramSettings settings, ObjectModel objectModel, RecordingEngine recorder, CombiningEngine combiner)
        {
            _settings = settings;
            Recorder = recorder;
            Combiner = combiner;

            _panels = new List<AbstractPanelViewModel>
            {
                new BarcodePanelViewModel(this, objectModel, _settings.ProjectCode) {Visibility = Visibility.Visible},
                new RecordPanelViewModel(this, objectModel) {Visibility = Visibility.Collapsed},
                new FinishPanelViewModel(this, objectModel) {Visibility = Visibility.Collapsed}
            };
        
        }

        public string Title => $"{_settings.ProjectCode} Recorder";
        
        public BarcodePanelViewModel BarcodePanelViewModel => GetPanel<BarcodePanelViewModel>();
        public RecordPanelViewModel RecordPanelViewModel => GetPanel<RecordPanelViewModel>();
        public FinishPanelViewModel FinishPanelViewModel => GetPanel<FinishPanelViewModel>();

        public AbstractPanelViewModel ActivePanelModel => _panels.Single(p => p.Visibility == Visibility.Visible);
        public RecordingEngine Recorder { get; set; }

        public CombiningEngine Combiner { get; set; }

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

        public void OnWindowClosing(object sender, CancelEventArgs args)
        {
            if (Recorder.Recording == false)
            {
                return;
            }

            var view = sender as UserControls;
            var result = view == null 
                ? MessageBox.Show("You are still recording. Are you sure you want to exit", "Stop Recording and Exit?", MessageBoxButton.YesNo, MessageBoxImage.Warning, MessageBoxResult.No) 
                : MessageBox.Show(view, "You are still recording. Are you sure you want to exit", "Stop Recording and Exit?", MessageBoxButton.YesNo, MessageBoxImage.Warning, MessageBoxResult.No);

            if (result != MessageBoxResult.Yes)
            {
                args.Cancel = true;
            }
        }
    }
}