using System;
using Packager.UserInterface;

namespace Packager.Observers
{
    internal class ViewModelObserver : IViewModelObserver
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

        public void BeginSection(Guid sectionKey, string baseMessage, params object[] elements)
        {
            ViewModel.BeginSection(sectionKey, string.Format(baseMessage, elements));
        }

        public void EndSection(Guid sectionKey)
        {
            ViewModel.EndSection(sectionKey);
           
        }

        public void FlagAsSuccessful(Guid sectionKey, string newTitle)
        {
            ViewModel.FlagSectionAsSuccessful(sectionKey, newTitle);
        }

        public void FlagAsWarning(Guid sectionKey, string newTitle)
        {
            throw new NotImplementedException();
        }

        public void FlagAsError(Guid sectionKey, string newTitle)
        {
            throw new NotImplementedException();
        }
    }
}