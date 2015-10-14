﻿using System;
using System.Windows;
using Recorder.ViewModels;

namespace Recorder.Dialogs
{
    /// <summary>
    ///     Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class UserControls : Window
    {
        public UserControls()
        {
            InitializeComponent();
        }

        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);

            var viewModel = DataContext as UserControlsViewModel;
            if (viewModel == null)
            {
                return;
            }

            //viewModel.HookEvents(this);
            
        }

        private void OnWindowClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            var onClosing = DataContext as IClosing;
            if (onClosing == null)
            {
                return;
            }

            e.Cancel = onClosing.CancelWindowClose();
        }
    }
}