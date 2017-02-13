using System.Text.RegularExpressions;
using ICSharpCode.AvalonEdit.Rendering;

namespace Common.UserInterface.LineGenerators
{
    public abstract class AbstractBarcodeElementGenerator : VisualLineElementGenerator
    {
        private readonly Regex _barCodeRegEx = new Regex(@"\d{14}");

        public override int GetFirstInterestedOffset(int startOffset)
        {
            int matchOffset;
            GetMatch(startOffset, out matchOffset);
            return matchOffset;
        }

        protected Match GetMatch(int startOffset, out int matchOffset)
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

        public abstract override VisualLineElement ConstructElement(int offset);

    }
}