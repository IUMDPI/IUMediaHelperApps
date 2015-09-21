using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
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
        private readonly RecordingEngine _recorder;
        private readonly CombiningEngine _combiner;

        public UserControlsViewModel(IProgramSettings settings, ObjectModel objectModel, RecordingEngine recorder, CombiningEngine combiner)
        {
            _settings = settings;
            _recorder = recorder;
            _combiner = combiner;

            _panels = new List<AbstractPanelViewModel>
            {
                new BarcodePanelViewModel(this, objectModel, _settings.ProjectCode) {Visibility = Visibility.Visible},
                new RecordPanelViewModel(this, objectModel, _recorder) {Visibility = Visibility.Collapsed},
                new FinishPanelViewModel(this, objectModel, _combiner) {Visibility = Visibility.Collapsed}
            };
            ActionPanelViewModel = new ActionPanelViewModel(this);
        }

        public string Title => $"{_settings.ProjectCode} Recorder";
        
        public ActionPanelViewModel ActionPanelViewModel { get; private set; }
        public BarcodePanelViewModel BarcodePanelViewModel => GetPanel<BarcodePanelViewModel>();
        public RecordPanelViewModel RecordPanelViewModel => GetPanel<RecordPanelViewModel>();
        public FinishPanelViewModel FinishPanelViewModel => GetPanel<FinishPanelViewModel>();

        public AbstractPanelViewModel ActivePanelModel => _panels.Single(p => p.Visibility == Visibility.Visible);

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

        public void ShowPanel<T>() where T : AbstractPanelViewModel
        {
            foreach (var panel in _panels)
            {
                panel.Initialize();
                panel.Visibility = panel.GetType() == typeof (T) ? Visibility.Visible : Visibility.Collapsed;
            }
            OnPropertyChanged(nameof(ActivePanelModel));
        }

        public void OnWindowClosing(object sender, CancelEventArgs args)
        {
            if (_recorder.Recording == false)
            {
                return;
            }
            var result = MessageBox.Show("You are still recording. Are you sure you want to exit", "Stop Recording and Exit", MessageBoxButton.YesNo);
            if (result != MessageBoxResult.Yes)
            {
                args.Cancel = true;
            }
        }
    }
}