using Common.UserInterface.LineGenerators;
using Common.UserInterface.ViewModels;
using ICSharpCode.AvalonEdit.Rendering;

namespace Packager.UserInterface.LineGenerators
{
    public class BarcodeElementGenerator:AbstractBarcodeElementGenerator
    {
        public BarcodeElementGenerator(ILogPanelViewModel viewModel)
        {
            ViewModel = viewModel;
        }

        public override VisualLineElement ConstructElement(int offset)
        {
            int matchOffset;
            var match = GetMatch(offset, out matchOffset);
            return match.Success
                ? new BarCodeLinkText(match.Value, CurrentContext.VisualLine, ViewModel)
                : null;
        }

        private ILogPanelViewModel ViewModel { get;  }
    }
}
