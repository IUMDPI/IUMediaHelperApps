using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using Common.Annotations;
using Common.Exceptions;
using Common.Extensions;
using Common.UserInterface.LineGenerators;
using ICSharpCode.AvalonEdit;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Editing;
using ICSharpCode.AvalonEdit.Folding;

namespace Common.UserInterface.ViewModels
{
    public interface ILogPanelViewModel
    {
        void BeginSection(string sectionKey, string text);
        void EndSection(string sectionKey, string newTitle = "", bool collapse = false);
        void Initialize(TextEditor textEditor);
        void InsertLine(string value);
        void LogError(Exception e);
        void ScrollToSection(string sectionKey);
        void Clear();
    }

    public class LogPanelViewModel : ILogPanelViewModel, INotifyPropertyChanged
    {
        private TextDocument _document;

        public TextDocument Document
        {
            get { return _document; }
            private set { _document = value; OnPropertyChanged(); }
        }

        private bool _autoScroll;

        private int TextLength => Document.TextLength;
        private TextArea TextArea { get; set; }
        private TextEditor TextEditor { get; set; }
        private FoldingManager FoldManager { get; set; }

        private bool AutoScroll
        {
            get { return _autoScroll; }
            set
            {
                _autoScroll = value;
                OnPropertyChanged();
            }
        }

        public LogPanelViewModel()
        {
            Document = new TextDocument();
        }

        private readonly List<SectionModel> _sections = new List<SectionModel>();

        public void BeginSection(string sectionKey, string text)
        {
            if (sectionKey.IsNotSet())
            {
                return;
            }

            if (text.IsNotSet())
            {
                return;
            }

            if (Document.Text.EndsWith("\n\n") == false)
            {
                InsertLine();
            }

            var sectionModel = GetOrCreateSectionModel(sectionKey);

            sectionModel.StartOffset = TextLength;
            sectionModel.Title = text;

            InsertLine(text);
            InsertLine();
        }

        public void EndSection(string sectionKey, string newTitle = "", bool collapse = false)
        {
            if (sectionKey.IsNotSet())
            {
                return;
            }

            var sectionModel = _sections.SingleOrDefault(m => m.Key.Equals(sectionKey));
            if (sectionModel == null)
            {
                return;
            }

            InsertLine();

            sectionModel.EndOffset = TextLength - 1;
            sectionModel.Completed = true;

            if (newTitle.IsSet())
            {
                sectionModel.Title = newTitle;
            }

            var foldingSection = FoldManager.CreateFolding(sectionModel.StartOffset, sectionModel.EndOffset);
            foldingSection.Title = Indent(sectionModel.Title, sectionModel.Indent);
            foldingSection.Tag = sectionModel;
            foldingSection.IsFolded = collapse;

            InsertLine();
        }

        public void Initialize(TextEditor textEditor)
        {
            AutoScroll = true;
            TextEditor = textEditor;
            TextArea = textEditor.TextArea;

            TextEditor.TextArea.TextView.ElementGenerators.Clear();
            TextArea.TextView.ElementGenerators.Add(new BarcodeElementGenerator(this));

            TextArea.SelectionCornerRadius = 0;
            TextArea.SelectionBorder = new Pen(SystemColors.HighlightBrush, 1);
            TextArea.SelectionBrush = SystemColors.HighlightBrush;

            ((IScrollInfo)TextArea).ScrollOwner.ScrollChanged += ScrollChangedHandler;

            Document.PropertyChanged += DocumentPropertyChangedHandler;

            FoldManager = FoldingManager.Install(TextArea);
        }

        private void DocumentPropertyChangedHandler(object sender, PropertyChangedEventArgs e)
        {
            if (!e.PropertyName.Equals("LineCount"))
            {
                return;
            }

            if (!AutoScroll)
            {
                return;
            }

            var scrollTo = TextArea.TextView.GetVisualTopByDocumentLine(Document.LineCount);
            TextEditor.ScrollToVerticalOffset(scrollTo);
        }

        private void ScrollChangedHandler(object sender, ScrollChangedEventArgs e)
        {
            var viewer = sender as ScrollViewer;
            if (viewer == null)
            {
                return;
            }

            if (!e.ExtentHeightChange.Equals(0))
            {
                return;
            }

            AutoScroll = viewer.VerticalOffset.Equals(viewer.ScrollableHeight);
        }

        public void InsertLine(string value)
        {
            if (value.IsNotSet())
            {
                value = "";
            }

            value = value.TrimEnd('\n');

            var indent = GetIndents(value);
            var lines = value.Split('\n');

            foreach (var line in lines)
            {
                Document.Insert(TextLength, $"{Indent(line, indent)}\n");
            }
        }

        private void InsertLine()
        {
            Document.Insert(TextLength, "\n");
        }

        public void LogError(Exception e)
        {
            if (e is AbstractEngineException)
            {
                InsertLine();
                InsertLine($"ERROR: {e.Message}");
                InsertLine();
                return;
            }

            var sectionKey = Guid.NewGuid().ToString();
            BeginSection(sectionKey, $"ERROR: {e.Message}");
            InsertLine(e.StackTrace);
            EndSection(sectionKey);
        }

        private SectionModel GetOrCreateSectionModel(string key)
        {
            var sectionModel = _sections.SingleOrDefault(m => m.Key.Equals(key));
            if (sectionModel != null) return sectionModel;

            sectionModel = new SectionModel
            {
                Key = key,
                Indent = _sections.Count(m => !m.Completed)
            };

            _sections.Add(sectionModel);
            return sectionModel;
        }

        private int GetIndents(string value)
        {
            return _sections
                .Where(m => m.Completed == false)
                .Count(m => !m.Title.Equals(value));
        }

        private static string Indent(string value, int indent)
        {
            return indent == 0
                ? value
                : $"{new string(' ', indent * 2)}{value}";
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public void ScrollToSection(string sectionKey)
        {
            var section = _sections.FirstOrDefault(m => m.Key.Equals(sectionKey));
            if (section == null)
            {
                return;
            }

            var folding = FoldManager.AllFoldings.SingleOrDefault(f => f.Tag.Equals(section));
            if (folding == null)
            {
                return;
            }

            folding.IsFolded = false;

            var line = TextEditor.Document.GetLineByOffset(section.StartOffset);
            TextEditor.ScrollTo(line.LineNumber, 0);
        }

        public void Clear()
        {
            _sections.Clear();
            Document.Text = string.Empty;

        }
    }
}
