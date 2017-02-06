using System.Reflection;
using System.Xml;
using ICSharpCode.AvalonEdit;
using ICSharpCode.AvalonEdit.Highlighting;
using ICSharpCode.AvalonEdit.Highlighting.Xshd;

namespace Common.UserInterface.Highlighting
{
    public static class Highlighting
    {
        public static void Configure(TextEditor textEditor)
        {
            var assembly = Assembly.GetExecutingAssembly();
            using (var s = assembly.GetManifestResourceStream("Common.UserInterface.Highlighting.HighlightRules.xshd"))
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
