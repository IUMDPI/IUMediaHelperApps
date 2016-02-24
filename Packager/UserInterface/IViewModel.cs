using System;

namespace Packager.UserInterface
{
    public interface IViewModel
    {
        void BeginSection(string sectionKey, string text);
        void EndSection(string sectionKey, string newTitle = "", bool collapse = false);
        void Initialize(OutputWindow outputWindow, string projectCode);
        void InsertLine(string value);
        void LogError(Exception e);
        void ScrollToBarcodeSection(string barCode);
        bool Processing { get; set; }
        string ProcessingMessage { get; set; }
    }
}