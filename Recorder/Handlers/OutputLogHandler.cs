using System.Diagnostics;
using System.Text;
using Recorder.ViewModels;

namespace Recorder.Handlers
{
    public class OutputLogHandler: AbstractProcessOutputHandler
    {
        public OutputLogHandler(OutputWindowViewModel viewModel)
        {
            ViewModel = viewModel;
        }

        private OutputWindowViewModel ViewModel { get; }

        public override void OnDataReceived(object sender, DataReceivedEventArgs args)
        {
            if (string.IsNullOrWhiteSpace(args.Data))
            {
                return;
            }

            AppendLine(args.Data);
        }

        private void AppendLine(string text)
        {
            var builder = new StringBuilder(ViewModel.Text);
            builder.AppendLine(text);
            ViewModel.Text = builder.ToString();
        }

        public override void Reset()
        {
            if (ViewModel.Clear == false)
            {
                AppendLine("");
                AppendLine("------------------------------------------------------------------------");
                return;
            }

            ViewModel.Text = string.Empty;
        }
    }
}