using System;
using System.Collections.Generic;
using ICSharpCode.AvalonEdit;
using ICSharpCode.AvalonEdit.Rendering;

namespace Common.UserInterface.ViewModels
{
    public interface ILogPanelViewModel
    {
        void BeginSection(string sectionKey, string text);
        void EndSection(string sectionKey, string newTitle = "", bool collapse = false);
        void Initialize(TextEditor textEditor, IEnumerable<VisualLineElementGenerator> lineElementGenerators);
        void InsertLine(string value);
        void LogError(Exception e);
        void ScrollToSection(string sectionKey);
        void Clear();
    }
}
