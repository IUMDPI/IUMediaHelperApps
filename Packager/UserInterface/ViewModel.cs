using System;
using System.Collections.Generic;
using System.Text;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Folding;

namespace Packager.UserInterface
{
    public class ViewModel
    {
        private readonly List<NewFolding> _foldings = new List<NewFolding>();
        private FoldingManager _foldingManager;

        public TextDocument Document { get; private set; }

        private FoldingManager FoldingManager
        {
            get { return _foldingManager; }
            set { _foldingManager = value; }
        }

        public ViewModel()
        {
            Document = new TextDocument();
            Document.Changed += DocumentChangedHandler;
            Document.TextChanged += TextChangedHandler;
        }

        public void Initialize(OutputWindow outputWindow)
        {
            outputWindow.DataContext = this;
            outputWindow.Show();
            FoldingManager = FoldingManager.Install(outputWindow.OutputText.TextArea);
            
        }

        private void TextChangedHandler(object sender, EventArgs e)
        {
            FoldingManager.UpdateFoldings(_foldings, -1);
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
            
            var folding = new NewFolding(TextLength, TextLength + builder.Length-1);
            _foldings.Add(folding);

            Document.Insert(TextLength, builder.ToString());
        }

        private int TextLength
        {
            get { return Document.TextLength; }
        }
    }
}