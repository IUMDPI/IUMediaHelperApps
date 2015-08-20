using System.Text.RegularExpressions;
using ICSharpCode.AvalonEdit.Rendering;

namespace Packager.UserInterface
{
    public class BarcodeElementGenerator : VisualLineElementGenerator
    {
        private readonly Regex _barCodeRegEx = new Regex(@"\d{14}");

        public BarcodeElementGenerator(ViewModel viewModel)
        {
            ViewModel = viewModel;
        }

        private ViewModel ViewModel { get; }

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