using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Folding;
using Packager.Exceptions;
using Packager.Models.UserInterfaceModels;

namespace Packager.UserInterface
{
    public class ViewModel
    {
        private readonly List<SectionModel> _sections = new List<SectionModel>();

        public ViewModel()
        {
            Document = new TextDocument();
            Document.Changed += DocumentChangedHandler;
        }

        public TextDocument Document { get; private set; }
        private FoldingManager FoldingManager { get; set; }

        private int TextLength
        {
            get { return Document.TextLength; }
        }

        public void Initialize(OutputWindow outputWindow)
        {
            outputWindow.DataContext = this;
            outputWindow.Show();

            FoldingManager = FoldingManager.Install(outputWindow.OutputText.TextArea);
        }

        private void DocumentChangedHandler(object sender, DocumentChangeEventArgs e)
        {
            var test = 0;
        }

        public void InsertLine(string value)
        {
            value = value.TrimEnd('\n');

            var indent = GetIndents(value);
            var lines = value.Split('\n');

            foreach (var line in lines)
            {
                Document.Insert(TextLength, string.Format("{0}\n", Indent(line, indent)));
            }
        }

        public void LogError(Exception e)
        {
            var sectionKey = Guid.NewGuid();
            BeginSection(sectionKey, string.Format("ERROR: {0}", e.Message));
            if (!(e is AbstractEngineException))
            {
                InsertLine(e.StackTrace);    
            }
            EndSection(sectionKey);
        }

        private int GetIndents(string value)
        {
            return _sections
                .Where(m => m.Completed == false)
                .Count(m => !m.Title.Equals(value));
        }

        public void BeginSection(Guid sectionKey, string text)
        {
            if (sectionKey == Guid.Empty)
            {
                return;
            }

            if (string.IsNullOrWhiteSpace(text))
            {
                return;
            }

            InsertLine("");

            var sectionModel = GetOrCreateSectionModel(sectionKey);

            sectionModel.StartOffset = TextLength;
            sectionModel.Title = text;

            InsertLine(text);
        }

        private SectionModel GetOrCreateSectionModel(Guid key)
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

        public void EndSection(Guid sectionKey,string newTitle="", bool collapse=false)
        {
            if (sectionKey == Guid.Empty)
            {
                return;
            }

            var sectionModel = _sections.SingleOrDefault(m => m.Key.Equals(sectionKey));
            if (sectionModel == null)
            {
                return;
            }

            InsertLine("");

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
        }
        
        private static string Indent(string value, int indent)
        {
            return string.Format("{0}{1}", new String(' ', indent * 2), value);
        }

        public void FlagSectionAsSuccessful(Guid key, string newTitle)
        {
            var section = FoldingManager.AllFoldings.SingleOrDefault(s => IsSection(s, key));
            if (section == null)
            {
                return;
            }

            var model = section.Tag as SectionModel;
            if (model == null)
            {
                return;
            }

            section.Title = Indent(newTitle, model.Indent);
            section.IsFolded = true;
        }

        private static bool IsSection(FoldingSection section, Guid key)
        {
            var model = section.Tag as SectionModel;
            if (model == null)
            {
                return false;
            }

            return model.Key.Equals(key);
        }

    }


    
}