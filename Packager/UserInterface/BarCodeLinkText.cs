using System.Windows;
using System.Windows.Input;
using System.Windows.Media.TextFormatting;
using ICSharpCode.AvalonEdit.Rendering;

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
}