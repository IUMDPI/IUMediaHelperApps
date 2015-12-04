using System.Reflection;
using System.Windows;
using System.Xml;
using ICSharpCode.AvalonEdit.Highlighting;
using ICSharpCode.AvalonEdit.Highlighting.Xshd;

namespace Packager.UserInterface
{
    /// <summary>
    ///     Interaction logic for OutputWindow.xaml
    /// </summary>
    public partial class OutputWindow : Window
    {
        public OutputWindow()
        {
            InitializeComponent();

            var assembly = Assembly.GetExecutingAssembly();
            using (var s = assembly.GetManifestResourceStream("Packager.UserInterface.HighlightRules.xshd"))
            {
                using (var reader = new XmlTextReader(s))
                {
                    OutputText.SyntaxHighlighting = HighlightingLoader.Load(reader, HighlightingManager.Instance);
                }
            }
        }
    }
}