using System;
using System.Diagnostics;
using System.Text;
using Recorder.Utilities;
using Recorder.ViewModels;

namespace Recorder.Handlers
{
    internal class OutputReceivedHandler 
    {
        public OutputReceivedHandler(OutputWindowViewModel viewModel) 
        {
            ViewModel = viewModel;
        }

        private OutputWindowViewModel ViewModel { get;  }

        public void OnDataReceived(object sender, DataReceivedEventArgs args)
        {
            if (string.IsNullOrWhiteSpace(args.Data))
            {
                return;
            }

            var builder = new StringBuilder(ViewModel.Text);
            builder.AppendLine(args.Data);
            ViewModel.Text = builder.ToString();
        }
    }
}