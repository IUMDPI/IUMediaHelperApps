﻿using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using JetBrains.Annotations;
using Recorder.Models;
using Recorder.Utilities;

namespace Recorder.ViewModels
{
    public abstract class AbstractPanelViewModel : INotifyPropertyChanged
    {
        private bool _touched;
        private Visibility _visibility;
        
        public abstract void Initialize();
        
        protected AbstractPanelViewModel(UserControlsViewModel parent, ObjectModel objectModel)
        {
            Parent = parent;
            ObjectModel = objectModel;

        }

        protected ObjectModel ObjectModel { get; set; }

        protected UserControlsViewModel Parent { get; set; }

        public bool Touched
        {
            get { return _touched; }
            set
            {
                _touched = value;
                OnPropertyChanged();
            }
        }

        public abstract string BackButtonText { get; }
        public abstract string NextButtonText { get; }
        public abstract string Instructions { get; }

        public abstract ICommand NextCommand { get; }

        public abstract ICommand BackCommand { get; }

        public Visibility Visibility
        {
            get { return _visibility; }
            set
            {
                _visibility = value;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void FlagTouched(object originalValue, object newValue)
        {
            if (Touched)
            {
                return;
            }

            Touched = originalValue != newValue;
        }

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}