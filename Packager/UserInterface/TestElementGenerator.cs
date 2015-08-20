using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.TextFormatting;
using ICSharpCode.AvalonEdit;
using ICSharpCode.AvalonEdit.Rendering;
using ICSharpCode.AvalonEdit.Utils;

namespace Packager.UserInterface
{
    public class BarCodeLinkText : VisualLineText
    {
        private string Barcode { get; set; }
        private ViewModel ViewModel { get; set; }

        public BarCodeLinkText(string barcode, VisualLine parentVisualLine, ViewModel viewModel) : base(parentVisualLine, barcode.Length)
        {
            Barcode = barcode;
            ViewModel = viewModel;
        }

        protected override void OnQueryCursor(QueryCursorEventArgs e)
        {
            e.Cursor = Cursors.Hand;
            e.Handled = true;
        }

        protected override void OnMouseDown(MouseButtonEventArgs e)
        {
            ViewModel.ScrollToBarcodeSection(Barcode);
            e.Handled = true;
        }

        public override TextRun CreateTextRun(int startVisualColumn, ITextRunConstructionContext context)
        {
            TextRunProperties.SetForegroundBrush(context.TextView.LinkTextForegroundBrush);
            TextRunProperties.SetBackgroundBrush(context.TextView.LinkTextBackgroundBrush);
            TextRunProperties.SetTextDecorations(TextDecorations.Underline);

            return base.CreateTextRun(startVisualColumn, context);
        }
    }

    public class TestElementGenerator : VisualLineElementGenerator
    {
        private ViewModel ViewModel { get; set; }

        private readonly Regex _barCodeRegEx = new Regex(@"\d{14}");

        public TestElementGenerator(ViewModel viewModel)
        {
            ViewModel = viewModel;
        }

        public override int GetFirstInterestedOffset(int startOffset)
        {
            int matchOffset;
            GetMatch(startOffset, out matchOffset);
            return matchOffset;
        }

        private Match GetMatch(int startOffset, out int matchOffset)
        {
            var endOffset = CurrentContext.VisualLine.LastDocumentLine.EndOffset;
            var relevantText = CurrentContext.GetText(startOffset, endOffset - startOffset);
            var m = _barCodeRegEx.Match(relevantText.Text, relevantText.Offset, relevantText.Count);

            if (m.Success == false)
            {
                matchOffset = -1;
                return m;
            }

            if (relevantText.Text.Trim().StartsWith(m.Value) == false)
            {
                matchOffset = -1;
                return m;
            }
            
            matchOffset = m.Index - relevantText.Offset + startOffset;
            return m;
        }

        public override VisualLineElement ConstructElement(int offset)
        {
            int matchOffset;
            var match = GetMatch(offset, out matchOffset);
            if (match.Success == false)
            {
                return null;
            }

            return new BarCodeLinkText(match.Value, CurrentContext.VisualLine, ViewModel);
        }


    }
}
