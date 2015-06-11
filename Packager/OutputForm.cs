using System;
using System.Windows.Forms;
using Packager.Engine;
using Packager.Observers;

namespace Packager
{
    public partial class OutputForm : Form
    {
        private readonly IEngine _engine;

        public OutputForm(IEngine engine)
        {
            // needs to come first
            InitializeComponent();

            // assign engine to variable
            _engine = engine;
        }

        private async void FormLoadHandler(object sender, EventArgs e)
        {
            _engine.AddObserver(new TextBoxOutputObserver(outputTextBox));
            outputTextBox.Cursor = Cursors.WaitCursor;
            await _engine.Start();
            outputTextBox.Cursor = Cursors.Default;
        }
    }
}