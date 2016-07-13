using System.Reflection;
using System.Threading;
using System.Windows;
using System.Xml;
using ICSharpCode.AvalonEdit.Highlighting;
using ICSharpCode.AvalonEdit.Highlighting.Xshd;
using Packager.Engine;
using Packager.Models.SettingsModels;

namespace Packager.UserInterface
{
    /// <summary>
    ///     Interaction logic for OutputWindow.xaml
    /// </summary>
    public partial class OutputWindow : Window
    {
        private IViewModel ViewModel { get; }
        private IProgramSettings ProgramSettings { get; }
        private IEngine Engine { get; }
        private CancellationTokenSource CancellationTokenSource { get; }
    
        public OutputWindow(IProgramSettings programSettings, IViewModel viewModel, IEngine engine, CancellationTokenSource cancellationTokenSource)
        {
            InitializeComponent();

            ProgramSettings = programSettings;
            Engine = engine;
            CancellationTokenSource = cancellationTokenSource;
            ViewModel = viewModel;

            DataContext = viewModel;

            var assembly = Assembly.GetExecutingAssembly();
            using (var s = assembly.GetManifestResourceStream("Packager.UserInterface.HighlightRules.xshd"))
            {
                using (var reader = new XmlTextReader(s))
                {
                    OutputText.SyntaxHighlighting = HighlightingLoader.Load(reader, HighlightingManager.Instance);
                }
            }

            
        }

        private async void OnWindowLoaded(object sender, RoutedEventArgs e)
        {
            ViewModel.Initialize(this, ProgramSettings.ProjectCode);
            await Engine.Start(CancellationTokenSource.Token);
        }
    }
}