using System.Dynamic;
using Common.UserInterface.LineGenerators;
using ICSharpCode.AvalonEdit.Rendering;
using Reporter.Models;

namespace Reporter.LineGenerators
{
    public class OpenObjectLogfileGenerator:AbstractBarcodeElementGenerator
    {
        private ProgramSettings ProgramSettings { get; }

        public OpenObjectLogfileGenerator(ProgramSettings programSettings)
        {
            ProgramSettings = programSettings;
        }

        public override VisualLineElement ConstructElement(int offset)
        {
            int matchOffset;
            var match = GetMatch(offset, out matchOffset);
            return match.Success
                ? new OpenObjectLogFileLinkText(ProgramSettings.ReportFolder, match.Value, CurrentContext.VisualLine)
                : null;
        }
    }
}
