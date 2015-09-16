using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Recorder.ViewModels
{
    public class ActionPanelViewModel
    {
        private readonly UserControlsViewModel _parent;
        private RelayCommand _startPanelCommand;
        private ICommand _recordPanelCommand;
        private ICommand _finishPanelCommand;

        public ActionPanelViewModel(UserControlsViewModel parent)
        {
            _parent = parent;
        }

        public ICommand StartPanelCommand
        {
            get
            {
                return _startPanelCommand
                  ?? (_startPanelCommand = new RelayCommand(param => _parent.ShowPanel<BarcodePanelViewModel>(), param => true));
            }
        }

        public ICommand RecordPanelCommand
        {
            get
            {
                return _recordPanelCommand
                  ?? (_recordPanelCommand = new RelayCommand(param => _parent.ShowPanel<RecordPanelViewModel>(), param => true));
            }
        }


        public ICommand FinishPanelCommand
        {
            get
            {
                return _finishPanelCommand
                  ?? (_finishPanelCommand = new RelayCommand(param => _parent.ShowPanel<FinishPanelViewModel>(), param => true));
            }
        }
    }
}
