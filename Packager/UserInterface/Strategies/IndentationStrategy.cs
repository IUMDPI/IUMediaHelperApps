using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Folding;
using ICSharpCode.AvalonEdit.Indentation;

namespace Packager.UserInterface.Strategies
{
    public class IndentationStrategy 
    {
        private readonly TextDocument _document;
        private readonly List<NewFolding> _foldings;

        public IndentationStrategy(TextDocument document, List<NewFolding> foldings)
        {
            _document = document;
            _foldings = foldings;
        }

        public void IndentContent()
        {
            var builder = new StringBuilder();

            foreach (var line in _document.Lines)
            {
                var indent = new string(' ', GetFoldingsForOffset(line.Offset));
                builder.AppendLine(indent + line);
            }

            _document.Text = builder.ToString();
        }

        private int GetFoldingsForOffset(int offset)
        {
            return _foldings.Count(f => IsInFolding(offset, f));
        }

        private bool IsInFolding(int value, NewFolding folding)
        {
            return value >= folding.StartOffset && value <= folding.EndOffset;
        }


    }
}