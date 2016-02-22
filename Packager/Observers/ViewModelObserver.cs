using System;
using Packager.Exceptions;
using Packager.UserInterface;

namespace Packager.Observers
{
    internal class ViewModelObserver : IObserver
    {
        public ViewModelObserver(IViewModel viewModel)
        {
            ViewModel = viewModel;
        }

        private IViewModel ViewModel { get; }

        public void Log(string baseMessage, params object[] elements)
        {
            ViewModel.InsertLine(string.Format(baseMessage, elements));
        }

        public void LogProcessingError(Exception issue, string barcode)
        {
            LogEngineError(issue);
        }

        public void LogEngineError(Exception issue)
        {
            if (issue is LoggedException)
            {
                return;
            }

            ViewModel.LogError(issue);
        }

        public int UniqueIdentifier => 3;

        public void BeginSection(string sectionKey, string baseMessage, params object[] elements)
        {
            ViewModel.BeginSection(sectionKey, string.Format(baseMessage, elements));
        }

        public void EndSection(string sectionKey, string newTitle = "", bool collapse = true)
        {
            ViewModel.EndSection(sectionKey, newTitle, collapse);
        }
    }
}