using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Folding;
using Packager.Extensions;

namespace Packager.UserInterface
{
    public class ViewModel
    {
        private readonly List<NewFolding> _foldings = new List<NewFolding>();

        private readonly Dictionary<Guid, NewFolding> _sectionDictionary =
            new Dictionary<Guid, NewFolding>();

        public ViewModel()
        {
            Document = new TextDocument();
            Document.Changed += DocumentChangedHandler;
            Document.TextChanged += TextChangedHandler;
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

        private void TextChangedHandler(object sender, EventArgs e)
        {
            FoldingManager.UpdateFoldings(_foldings.OrderBy(f => f.StartOffset), -1);
        }

        private void DocumentChangedHandler(object sender, DocumentChangeEventArgs e)
        {
            var test = 0;
        }

        public void InsertLine(string value)
        {
            Document.Insert(TextLength, string.Format("{0}\n", value));
        }

        public void InsertFolding(string value)
        {
            var builder = new StringBuilder();
            foreach (var line in value.Split('\n'))
            {
                builder.AppendFormat("\t{0}\n", line);
            }

            var folding = new NewFolding(TextLength, TextLength + builder.Length - 1)
            {
                Name = value.Split('\n').FirstOrDefault().ToDefaultIfEmpty()
            };

            //_foldings.Add(folding);

            Document.Insert(TextLength, builder.ToString());
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

            var folding = new NewFolding(TextLength, TextLength + 1)
            {
                Name = text
            };

            _sectionDictionary[sectionKey] = folding;
        }

        public void EndSection(Guid sectionKey)
        {
            if (sectionKey == Guid.Empty)
            {
                return;
            }

            if (!_sectionDictionary.ContainsKey(sectionKey))
            {
                return;
            }

            var folding = _sectionDictionary[sectionKey];
            folding.EndOffset = TextLength;
            _foldings.Add(folding);
        }
    }
}