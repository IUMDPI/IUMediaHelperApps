using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using ICSharpCode.AvalonEdit;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Editing;
using ICSharpCode.AvalonEdit.Folding;
using Packager.Annotations;
using Packager.Exceptions;
using Packager.Extensions;
using Packager.Models.UserInterfaceModels;

namespace Packager.UserInterface
{
    public class ViewModel : INotifyPropertyChanged, IViewModel, IDisposable
    {
        private readonly CancellationTokenSource _cancellationTokenSource;
        private readonly List<SectionModel> _sections = new List<SectionModel>();
        private RelayCommand _cancelCommand;
        private bool _processing;
        private string _processingMessage;
        private string _title;

        public ViewModel(CancellationTokenSource cancellationTokenSource)
        {
            _cancellationTokenSource = cancellationTokenSource;
            Document = new TextDocument();
        }

        public string Title
        {
            get { return _title; }
            set
            {
                _title = value;
                OnPropertyChanged();
            }
        }

        private bool AutoScroll { get; set; }

        public TextDocument Document { get; }

        public RelayCommand CancelCommand
        {
            get
            {
                return _cancelCommand ?? (_cancelCommand = new RelayCommand(param => DoCancel(), param => Processing));
            }
        }

        private FoldingManager FoldingManager { get; set; }

        private int TextLength => Document.TextLength;

        private TextArea TextArea => TextEditor.TextArea;

        private TextEditor TextEditor { get; set; }

        public void Dispose()
        {
            _cancellationTokenSource?.Dispose();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public string ProcessingMessage
        {
            get { return _processingMessage; }
            set
            {
                _processingMessage = value;
                OnPropertyChanged();
            }
        }

        public bool Processing
        {
            get { return _processing; }
            set
            {
                Debug.Print($"processing set to {value}");
                _processing = value;
                OnPropertyChanged();
                CancelCommand?.RaiseCanExecuteChanged();
            }
        }

        public void Initialize(OutputWindow outputWindow, string projectCode)
        {
            AutoScroll = true;
            TextEditor = outputWindow.OutputText;

            // clear default element generators
            TextEditor.TextArea.TextView.ElementGenerators.Clear();
            TextArea.TextView.ElementGenerators.Add(new BarcodeElementGenerator(this));

            // set selection formatting
            TextArea.SelectionCornerRadius = 0;
            TextArea.SelectionBorder = new Pen(SystemColors.HighlightBrush, 1);
            TextArea.SelectionBrush = SystemColors.HighlightBrush;

            ((IScrollInfo) outputWindow.OutputText.TextArea).ScrollOwner.ScrollChanged += ScrollChangedHandler;

            Document.PropertyChanged += DocumentPropertyChangedHandler;
            Title = $"{projectCode.ToUpperInvariant()} Media Packager";

            FoldingManager = FoldingManager.Install(TextArea);
        }

        public void InsertLine(string value)
        {
            value = value.TrimEnd('\n');

            var indent = GetIndents(value);
            var lines = value.Split('\n');

            foreach (var line in lines)
            {
                Document.Insert(TextLength, $"{Indent(line, indent)}\n");
            }
        }

        public void ScrollToBarcodeSection(string barCode)
        {
            var section = _sections.FirstOrDefault(m => m.Key.Equals(barCode));
            if (section == null)
            {
                return;
            }

            var folding = FoldingManager.AllFoldings.SingleOrDefault(f => f.Tag.Equals(section));
            if (folding == null)
            {
                return;
            }

            folding.IsFolded = false;

            var line = TextEditor.Document.GetLineByOffset(section.StartOffset);
            TextEditor.ScrollTo(line.LineNumber, 0);
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

            var foldingSection = FoldingManager.CreateFolding(sectionModel.StartOffset, sectionModel.EndOffset);
            foldingSection.Title = Indent(sectionModel.Title, sectionModel.Indent);
            foldingSection.Tag = sectionModel;
            foldingSection.IsFolded = collapse;

            InsertLine();
        }

        private void DoCancel()
        {
            try
            {
                _cancellationTokenSource.Cancel(true);
            }
            catch (OperationCanceledException)
            {
                // swallow issue here - it will be reported elsewhere
            }
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

        private void InsertLine()
        {
            Document.Insert(TextLength, "\n");
        }

        private int GetIndents(string value)
        {
            return _sections
                .Where(m => m.Completed == false)
                .Count(m => !m.Title.Equals(value));
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
        
        private static string Indent(string value, int indent)
        {
            return indent == 0
                ? value
                : $"{new string(' ', indent*2)}{value}";
        }

        [NotifyPropertyChangedInvocator]
        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            var handler = PropertyChanged;
            handler?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}