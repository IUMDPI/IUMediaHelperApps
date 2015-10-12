﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using ICSharpCode.AvalonEdit;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Editing;
using ICSharpCode.AvalonEdit.Folding;
using Packager.Annotations;
using Packager.Exceptions;
using Packager.Models;
using Packager.Models.UserInterfaceModels;

namespace Packager.UserInterface
{
    public class ViewModel : INotifyPropertyChanged
    {
        private readonly List<SectionModel> _sections = new List<SectionModel>();
        private string _title;

        public ViewModel()
        {
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

        private FoldingManager FoldingManager { get; set; }

        private int TextLength => Document.TextLength;

        private TextArea TextArea => TextEditor.TextArea;

        private TextEditor TextEditor { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        public void Initialize(OutputWindow outputWindow, IProgramSettings programSettings)
        {
            AutoScroll = true;
            TextEditor = outputWindow.OutputText;

            // clear default element generators
            TextEditor.TextArea.TextView.ElementGenerators.Clear();
            TextArea.TextView.ElementGenerators.Add(new BarcodeElementGenerator(this));

            outputWindow.DataContext = this;
            outputWindow.Show();

            ((IScrollInfo) outputWindow.OutputText.TextArea).ScrollOwner.ScrollChanged += ScrollChangedHandler;

            Document.PropertyChanged += DocumentPropertyChangedHandler;
            Title = $"{programSettings.ProjectCode.ToUpperInvariant()} Media Packager";

            FoldingManager = FoldingManager.Install(TextArea);

            var test = TextEditor.TextArea.TextView.Services;
            //TextArea.TextView.ElementGenerators.Clear();
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
                InsertLine($"ERROR: {e.Message}");
                return;
            }

            var sectionKey = Guid.NewGuid().ToString();
            BeginSection(sectionKey, $"ERROR: {e.Message}");
            InsertLine(e.StackTrace);
            EndSection(sectionKey);
        }

        private int GetIndents(string value)
        {
            return _sections
                .Where(m => m.Completed == false)
                .Count(m => !m.Title.Equals(value));
        }

        public void BeginSection(string sectionKey, string text)
        {
            if (string.IsNullOrEmpty(sectionKey))
            {
                return;
            }

            if (string.IsNullOrWhiteSpace(text))
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

        public void EndSection(string sectionKey, string newTitle = "", bool collapse = false)
        {
            if (string.IsNullOrEmpty(sectionKey))
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

            if (string.IsNullOrWhiteSpace(newTitle) == false)
            {
                sectionModel.Title = newTitle;
            }

            var foldingSection = FoldingManager.CreateFolding(sectionModel.StartOffset, sectionModel.EndOffset);
            foldingSection.Title = Indent(sectionModel.Title, sectionModel.Indent);
            foldingSection.Tag = sectionModel;
            foldingSection.IsFolded = collapse;

            InsertLine();
        }

        private static string Indent(string value, int indent)
        {
            return indent == 0
                ? value
                : $"{new string(' ', indent*2)}{value}";
        }

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            var handler = PropertyChanged;
            handler?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}