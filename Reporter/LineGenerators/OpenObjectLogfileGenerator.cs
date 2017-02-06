using Common.UserInterface.LineGenerators;
using ICSharpCode.AvalonEdit.Rendering;

namespace Reporter.LineGenerators
{
    public class OpenObjectLogfileGenerator:AbstractBarcodeElementGenerator
    {
        public override VisualLineElement ConstructElement(int offset)
        {
            int matchOffset;
            var match = GetMatch(offset, out matchOffset);
            return match.Success
                ? new OpenObjectLogFileLinkText(match.Value, CurrentContext.VisualLine)
                : null;
        }
    }
}
