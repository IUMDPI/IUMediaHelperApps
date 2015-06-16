using Packager.UserInterface;

namespace Packager.Observers
{
    internal class ViewModelObserver : IObserver
    {
        public ViewModelObserver(ViewModel viewModel)
        {
            ViewModel = viewModel;
        }

        private ViewModel ViewModel { get; set; }

        public void Log(string baseMessage, params object[] elements)
        {
            ViewModel.InsertLine(string.Format(baseMessage, elements));
        }

        public void LogHeader(string baseMessage, params object[] elements)
        {
            Log(baseMessage, elements);
        }

        public void LogError(string baseMessage, object[] elements)
        {
            Log(baseMessage, elements);
        }

        public void LogExternal(string text)
        {
            ViewModel.InsertFolding(text);
        }
    }
}