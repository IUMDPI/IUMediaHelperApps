using System.Reflection;
using System.Xml;
using ICSharpCode.AvalonEdit;
using ICSharpCode.AvalonEdit.Highlighting;
using ICSharpCode.AvalonEdit.Highlighting.Xshd;

namespace Common.UserInterface.HighlightingRules
{
    public static class Configuration
    {
        public static void ApplyRules(TextEditor textEditor)
        {
            var assembly = Assembly.GetExecutingAssembly();
            using (var s = assembly.GetManifestResourceStream("Common.UserInterface.HighlightingRules.HighlightRules.xshd"))
            {
                if (s == null)
                {
                    return;
                }
                using (var reader = new XmlTextReader(s))
                {

                    textEditor.SyntaxHighlighting = HighlightingLoader.Load(reader, HighlightingManager.Instance);
                }
            }
        }
    }
}
